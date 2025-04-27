using MeetingManager.Models;
using MeetingManager.Services;
using MeetingManager.Services.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MeetingManager.Tests
{
  [TestFixture]
  public class MeetingServiceTests
  {
    private MeetingService _meetingService;
    private MeetingRepository _meetingRepository;
    private NotificationService _notificationService;
    private Meeting _validMeeting;
    private string _testExportFilePath;

    [SetUp]
    public void Setup()
    {
      _meetingRepository = new MeetingRepository();
      _notificationService = new NotificationService(_meetingRepository);
      _meetingService = new MeetingService(_meetingRepository, _notificationService);

      _validMeeting = new Meeting
      {
        Title = "Valid Meeting",
        StartTime = DateTime.Now.AddHours(1),
        EndTime = DateTime.Now.AddHours(2),
        NotificationTime = DateTime.Now.AddMinutes(30)
      };

      _testExportFilePath = Path.Combine(Path.GetTempPath(), "meetings_export_test.txt");
    }

    [TearDown]
    public void TearDown()
    {
      if (File.Exists(_testExportFilePath))
      {
        File.Delete(_testExportFilePath);
      }
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
      _meetingRepository = null;
      _notificationService.Dispose();
      _meetingService = null;
    }

    [Test]
    public void AddMeeting_ShouldAddValidMeeting()
    {
      // Act
      _meetingService.AddMeeting(_validMeeting);

      // Assert
      var result = _meetingService.GetMeetingById(_validMeeting.Id);
      Assert.That(result, Is.Not.Null);
      Assert.That(result.Title, Is.EqualTo(_validMeeting.Title));
    }

    [Test]
    public void AddMeeting_ShouldThrowWhenStartTimeInPast()
    {
      // Arrange
      var invalidMeeting = new Meeting
      {
        Title = "Past Meeting",
        StartTime = DateTime.Now.AddHours(-1),
        EndTime = DateTime.Now.AddHours(1)
      };

      // Act
      var result = () => _meetingService.AddMeeting(invalidMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void AddMeeting_ShouldThrowWhenEndTimeBeforeStartTime()
    {
      // Arrange
      var invalidMeeting = new Meeting
      {
        Title = "Invalid Time Meeting",
        StartTime = DateTime.Now.AddHours(2),
        EndTime = DateTime.Now.AddHours(1)
      };

      // Act
      var result = () => _meetingService.AddMeeting(invalidMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void AddMeeting_ShouldThrowWhenNotificationAfterStart()
    {
      // Arrange
      var invalidMeeting = new Meeting
      {
        Title = "Invalid Notification",
        StartTime = DateTime.Now.AddHours(1),
        EndTime = DateTime.Now.AddHours(2),
        NotificationTime = DateTime.Now.AddHours(1.5)
      };

      // Act
      var result = () => _meetingService.AddMeeting(invalidMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void AddMeeting_ShouldThrowWhenTimeConflictExists()
    {
      // Arrange
      _meetingService.AddMeeting(_validMeeting);
      var conflictingMeeting = new Meeting
      {
        Title = "Conflicting Meeting",
        StartTime = _validMeeting.StartTime.AddMinutes(30),
        EndTime = _validMeeting.EndTime.AddMinutes(40)
      };

      // Act
      var result = () => _meetingService.AddMeeting(conflictingMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void UpdateMeeting_ShouldUpdateExistingMeeting()
    {
      // Arrange
      _meetingService.AddMeeting(_validMeeting);
      var updatedMeeting = new Meeting
      {
        Id = _validMeeting.Id,
        Title = "Updated Title",
        StartTime = _validMeeting.StartTime,
        EndTime = _validMeeting.EndTime
      };

      // Act
      _meetingService.UpdateMeeting(updatedMeeting);

      // Assert
      var result = _meetingService.GetMeetingById(_validMeeting.Id);
      Assert.That(result.Title, Is.EqualTo("Updated Title"));
    }

    [Test]
    public void UpdateMeeting_ShouldThrowWhenMeetingNotFound()
    {
      // Arrange
      var nonExistentMeeting = new Meeting { Id = 999 };

      // Act
      var result = () => _meetingService.UpdateMeeting(nonExistentMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public void UpdateMeeting_ShouldThrowWhenMeetingAlreadyStarted()
    {
      // Arrange
      var pastMeeting = new Meeting
      {
        Id = 1,
        Title = "Past Meeting",
        StartTime = DateTime.Now.AddHours(-2),
        EndTime = DateTime.Now.AddHours(-1)
      };
      _meetingRepository.Add(pastMeeting);

      // Act
      var result = () => _meetingService.UpdateMeeting(pastMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void UpdateMeeting_ShouldValidateAllFields()
    {
      // Arrange
      _meetingService.AddMeeting(_validMeeting);
      var invalidMeeting = new Meeting
      {
        Id = _validMeeting.Id,
        Title = "Invalid Update",
        StartTime = DateTime.Now.AddHours(-1), 
        EndTime = DateTime.Now.AddHours(1)
      };

      // Act
      var result = () => _meetingService.UpdateMeeting(invalidMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void RemoveMeeting_ShouldRemoveExistingMeeting()
    {
      // Arrange
      _meetingService.AddMeeting(_validMeeting);

      // Act
      _meetingService.RemoveMeeting(_validMeeting.Id);

      // Assert
      Assert.Throws<KeyNotFoundException>(() => _meetingService.GetMeetingById(_validMeeting.Id));
    }

    [Test]
    public void RemoveMeeting_ShouldThrowWhenMeetingNotFound()
    {
      // Arrange 
      _meetingService.AddMeeting(_validMeeting);

      // Act
      var result = () => _meetingService.RemoveMeeting(999);

      // Assert
      Assert.That(result, Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public void GetMeetings_ShouldReturnAllMeetingsInOrder()
    {
      // Arrange
      var meeting1 = new Meeting 
      { 
        StartTime = DateTime.Now.AddMinutes(5),
        EndTime = DateTime.Now.AddMinutes(6)
      };
      var meeting2 = new Meeting
      {
        StartTime = DateTime.Now.AddMinutes(3),
        EndTime = DateTime.Now.AddMinutes(4)
      };

      _meetingService.AddMeeting(meeting1);
      _meetingService.AddMeeting(meeting2);

      // Act
      var result = _meetingService.GetMeetings();

      // Assert
      Assert.That(result, Has.Count.EqualTo(2));
      Assert.That(result[0].StartTime, Is.LessThan(result[1].StartTime));
    }

    [Test]
    public void GetMeetings_ShouldReturnEmptyListWhenNoMeetings()
    {
      // Act
      var result = _meetingService.GetMeetings();

      // Assert
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetMeetingsByDate_ShouldReturnOnlyMeetingsForSpecifiedDate()
    {
      // Arrange
      var today = DateTime.Today;
      var meeting1 = new Meeting
      {
        StartTime = DateTime.Now.AddMinutes(1),
        EndTime = DateTime.Now.AddMinutes(2)
      };
      var meeting2 = new Meeting
      {
        StartTime = DateTime.Now.AddDays(3),
        EndTime = DateTime.Now.AddDays(4)
      };
      _meetingService.AddMeeting(meeting1);
      _meetingService.AddMeeting(meeting2);

      // Act
      var result = _meetingService.GetMeetingsByDate(today);

      // Assert
      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(result[0].StartTime.Date, Is.EqualTo(today));
    }

    [Test]
    public void GetMeetingsByDate_ShouldReturnEmptyListWhenNoMeetingsForDate()
    {
      // Arrange
      var futureDate = DateTime.Today.AddYears(1);

      // Act
      var result = _meetingService.GetMeetingsByDate(futureDate);

      // Assert
      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetMeetingById_ShouldReturnCorrectMeeting()
    {
      // Arrange
      _meetingService.AddMeeting(_validMeeting);

      // Act
      var result = _meetingService.GetMeetingById(_validMeeting.Id);

      // Assert
      Assert.That(result, Is.EqualTo(_validMeeting));
    }

    [Test]
    public void GetMeetingById_ShouldThrowWhenMeetingNotFound()
    {
      // Arrange
      _meetingService.AddMeeting(_validMeeting);

      // Act
      var result = () => _meetingService.GetMeetingById(999);

      // Act & Assert
      Assert.That(result, Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public async Task ExportToFile_ShouldCreateFileWithMeetingData()
    {
      // Arrange
      var date = DateTime.Today;
      var now = DateTime.Now;
      var meeting = new Meeting
      {
        StartTime = now.AddMinutes(1),
        EndTime = now.AddMinutes(2),
        Title = "Export Test Meeting"
      };
      _meetingService.AddMeeting(meeting);

      // Act
      await _meetingService.ExportToFile(date, _testExportFilePath);

      // Assert
      Assert.That(File.Exists(_testExportFilePath), Is.True);
      var content = await File.ReadAllTextAsync(_testExportFilePath);
      Assert.That(content, Contains.Substring("Export Test Meeting"));
    }

    [Test]
    public async Task ExportToFile_ShouldCreateEmptyFileWhenNoMeetings()
    {
      // Arrange
      var date = DateTime.Today;

      // Act
      await _meetingService.ExportToFile(date, _testExportFilePath);

      // Assert
      Assert.That(File.Exists(_testExportFilePath), Is.True);
      var content = await File.ReadAllTextAsync(_testExportFilePath);
      Assert.That(content, Is.Empty);
    }
  }
}