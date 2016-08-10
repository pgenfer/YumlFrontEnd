﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuml.Serializer.Dto;

namespace Yuml.Test.Serializer
{
    public class ToDomainConverterTest : TestBase
    {
        [TestDescription(
            @"Ensures that DTOs are converted back to domain objects and 
              relations between the original references are kept")]
        public void Classifiers_ConvertToDto_KeepReferences()
        {
            // Arrange
            var dtos = new List<ClassifierDto> { StringDto, IntegerDto };
            var converter = new DomainDtoConverter();
            // Act
            var classifierDictionary = converter.ToDomain(dtos);

            // ensure that properties use the same object references
            // as the classifier dictionary
            Assert.AreEqual(
                classifierDictionary.ElementAt(0), 
                classifierDictionary.ElementAt(1).Properties.ElementAt(0).Type);
            Assert.AreEqual(classifierDictionary.ElementAt(1),
                classifierDictionary.ElementAt(0).Properties.ElementAt(0).Type);
        }

        [TestDescription("Checks that methods are converted correctly between domain and dto")]
        public void Methods_ConvertToDtoAndDomain()
        {
            // Arrange
            var dtos = new List<ClassifierDto> {VoidDto, StringDto, ServiceDto};
            var converter = new DomainDtoConverter();
            // Act
            var classifierDictionary = converter.ToDomain(dtos);
            // Assert return type has correct reference
            Assert.AreEqual(
                classifierDictionary.ElementAt(0),
                classifierDictionary.ElementAt(2).Methods.ElementAt(0).ReturnType);
            // Assert parameters have correct reference
            Assert.AreEqual(
                classifierDictionary.ElementAt(1),
                classifierDictionary
                    .ElementAt(2).Methods
                    .ElementAt(0).Parameters
                    .ElementAt(0).Type);
        }
    }
}