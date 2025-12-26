using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using AnimalLibrary;

namespace ReflectionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Генерация XML диаграммы классов...");
            
            Assembly assembly = typeof(Animal).Assembly; //сборка содержащая тип animal 
            
            XDocument xmlDocument = GenerateXmlForAssembly(assembly); //генерация файла на основе сборки 
            
            string fileName = "AnimalClassDiagram.xml";
            xmlDocument.Save(fileName);
            
            Console.WriteLine($"XML диаграмма классов сохранена в файл: {fileName}");
            Console.WriteLine("\nСодержимое файла:");
            Console.WriteLine(xmlDocument.ToString());
            
            DemonstrateAnimals();
        }

        static void DemonstrateAnimals()
        {
            Console.WriteLine("\n Демонстрация работы классов животных ");
            
            Animal cow = new Cow("Россия", false, "Корова");
            Animal lion = new Lion("Африка", true, "Лев");
            Animal pig = new Pig("США", false, "Свинья");
            
            Animal[] animals = { cow, lion, pig };
            
            foreach (var animal in animals)
            {
                Console.WriteLine($"\n{animal.WhatAnimal} {animal.Name}:");
                Console.WriteLine($"  Страна: {animal.Country}");
                Console.WriteLine($"  Прячется от других: {animal.HideFromOtherAnimals}");
                Console.WriteLine($"  Классификация: {animal.GetClassificationAnimal()}");
                Console.WriteLine($"  Любимая еда: {animal.GetFavouriteFood()}");
                Console.Write("  Приветствие: ");
                animal.SayHello();
            }
        }

        static XDocument GenerateXmlForAssembly(Assembly assembly) //анализ сборки 
        {
            var types = assembly.GetTypes()  //получаем все типы из сборки 
                .Where(t => t.Namespace == "AnimalLibrary")
                .OrderBy(t => t.Name)
                .ToList();

            XElement root = new XElement("ClassDiagram",
                new XElement("AssemblyInfo",
                    new XAttribute("Name", assembly.GetName().Name),
                    new XAttribute("Version", assembly.GetName().Version?.ToString() ?? "1.0.0.0"),
                    new XAttribute("Generated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                ),
                new XElement("Types")
            );

            foreach (var type in types) //для каждого типа создаем 
            {
                XElement typeElement = CreateTypeElement(type);
                root.Element("Types").Add(typeElement);
            }

            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        }

        static XElement CreateTypeElement(Type type) //анализ конкретного типа 
        {
            string elementName = type.IsEnum ? "Enum" :  //определяем что за тип 
                               type.IsClass ? (type.IsAbstract ? "AbstractClass" : "Class") : 
                               "Type";

            XElement typeElement = new XElement(elementName, //создаем элемент с основной информацией 
                new XAttribute("Name", type.Name),
                new XAttribute("FullName", type.FullName)
            );
            
            var commentAttribute = type.GetCustomAttribute<CommentAttribute>();
            if (commentAttribute != null)
            {
                typeElement.Add(new XElement("Comment", commentAttribute.Comment));
            }

            if (type.IsEnum) //обработка перечислений 
            {
                XElement valuesElement = new XElement("Values");
                foreach (var value in Enum.GetValues(type))
                {
                    valuesElement.Add(new XElement("Value", 
                        new XAttribute("Name", value.ToString()),
                        new XAttribute("Value", (int)value)
                    ));
                }
                typeElement.Add(valuesElement);
            }
            else if (type.IsClass) //обработка классов 
            {
                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    typeElement.Add(new XElement("BaseType", 
                        new XAttribute("Name", type.BaseType.Name),
                        new XAttribute("FullName", type.BaseType.FullName)
                    ));
                }
                
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                    .Where(p => p.DeclaringType == type)
                                    .ToList();
                
                if (properties.Any())
                {
                    XElement propertiesElement = new XElement("Properties");
                    foreach (var property in properties)
                    {
                        propertiesElement.Add(new XElement("Property",
                            new XAttribute("Name", property.Name),
                            new XAttribute("Type", GetTypeName(property.PropertyType)),
                            new XAttribute("Access", GetAccessModifier(property))
                        ));
                    }
                    typeElement.Add(propertiesElement);
                }
                
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                 .Where(m => !m.IsSpecialName && m.DeclaringType == type)
                                 .ToList();
                
                if (methods.Any())
                {
                    XElement methodsElement = new XElement("Methods");
                    foreach (var method in methods)
                    {
                        var methodElement = new XElement("Method",
                            new XAttribute("Name", method.Name),
                            new XAttribute("ReturnType", GetTypeName(method.ReturnType)),
                            new XAttribute("IsAbstract", method.IsAbstract),
                            new XAttribute("IsVirtual", method.IsVirtual)
                        );
                        
                        var parameters = method.GetParameters();
                        if (parameters.Any())
                        {
                            XElement paramsElement = new XElement("Parameters");
                            foreach (var param in parameters)
                            {
                                paramsElement.Add(new XElement("Parameter",
                                    new XAttribute("Name", param.Name),
                                    new XAttribute("Type", GetTypeName(param.ParameterType))
                                ));
                            }
                            methodElement.Add(paramsElement);
                        }

                        methodsElement.Add(methodElement);
                    }
                    typeElement.Add(methodsElement);
                }
            }

            return typeElement;
        }

        static string GetTypeName(Type type)
        {
            if (type == null) return "void";
            
            if (type.IsGenericType)
            {
                string name = type.Name.Split('`')[0];
                var args = type.GetGenericArguments().Select(GetTypeName);
                return $"{name}<{string.Join(", ", args)}>";
            }
            
            return type.Name;
        }

        static string GetAccessModifier(PropertyInfo property) //модификаторы доступа 
        {
            if (property.GetMethod == null) return "write-only";
            if (property.SetMethod == null) return "read-only";
            
            var getAccess = property.GetMethod.IsPublic ? "public" : 
                           property.GetMethod.IsPrivate ? "private" : 
                           property.GetMethod.IsFamily ? "protected" : "internal";
            
            var setAccess = property.SetMethod.IsPublic ? "public" : 
                           property.SetMethod.IsPrivate ? "private" : 
                           property.SetMethod.IsFamily ? "protected" : "internal";
            
            return $"{getAccess} get; {setAccess} set;";
        }
    }
}
