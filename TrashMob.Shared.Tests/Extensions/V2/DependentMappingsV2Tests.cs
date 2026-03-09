namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using Xunit;

    public class DependentMappingsV2Tests
    {
        [Fact]
        public void Dependent_ToV2Dto_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var dob = new DateOnly(2015, 6, 15);

            var entity = new Dependent
            {
                Id = id,
                ParentUserId = parentId,
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = dob,
                Relationship = "Child",
                MedicalNotes = "Allergy to bee stings",
                EmergencyContactPhone = "555-0100",
                IsActive = true,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(id, dto.Id);
            Assert.Equal(parentId, dto.ParentUserId);
            Assert.Equal("Jane", dto.FirstName);
            Assert.Equal("Doe", dto.LastName);
            Assert.Equal(dob, dto.DateOfBirth);
            Assert.Equal("Child", dto.Relationship);
            Assert.Equal("Allergy to bee stings", dto.MedicalNotes);
            Assert.Equal("555-0100", dto.EmergencyContactPhone);
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void DependentDto_ToEntity_MapsAllProperties()
        {
            var id = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var dob = new DateOnly(2012, 3, 21);

            var dto = new DependentDto
            {
                Id = id,
                ParentUserId = parentId,
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = dob,
                Relationship = "Stepchild",
                MedicalNotes = "None",
                EmergencyContactPhone = "555-0200",
                IsActive = false,
            };

            var entity = dto.ToEntity();

            Assert.Equal(id, entity.Id);
            Assert.Equal(parentId, entity.ParentUserId);
            Assert.Equal("John", entity.FirstName);
            Assert.Equal("Smith", entity.LastName);
            Assert.Equal(dob, entity.DateOfBirth);
            Assert.Equal("Stepchild", entity.Relationship);
            Assert.Equal("None", entity.MedicalNotes);
            Assert.Equal("555-0200", entity.EmergencyContactPhone);
            Assert.False(entity.IsActive);
        }

        [Fact]
        public void Dependent_ToV2Dto_HandlesNullStrings()
        {
            var entity = new Dependent
            {
                Id = Guid.NewGuid(),
                ParentUserId = Guid.NewGuid(),
                FirstName = null,
                LastName = null,
                Relationship = null,
                MedicalNotes = null,
                EmergencyContactPhone = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.FirstName);
            Assert.Equal(string.Empty, dto.LastName);
            Assert.Equal(string.Empty, dto.Relationship);
            Assert.Equal(string.Empty, dto.MedicalNotes);
            Assert.Equal(string.Empty, dto.EmergencyContactPhone);
        }
    }
}
