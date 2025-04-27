using MeetingManager.Models;
using MeetingManager.Services.Interfaces;
using MeetingManager.Services.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetingManager.Tests
{
  [TestFixture]
  public class MeetingRepositoryTests
  {
    private MeetingRepository _meetingRepository;
    private Meeting _testMeeting;
    private List<Meeting> _testMeetings;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
      _testMeeting = new Meeting
      {
        Title = "Test Meeting",
        StartTime = DateTime.Now.AddHours(1),
        EndTime = DateTime.Now.AddHours(2),
        NotificationTime = DateTime.Now.AddMinutes(30)
      };
    }

    [SetUp]
    public void Setup()
    {
      _meetingRepository = new MeetingRepository();
    }

    [TearDown]
    public void Teardown()
    {
      foreach (var meeting in _meetingRepository.GetAll())
      {
        _meetingRepository.Remove(meeting.Id);
      }
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
      _meetingRepository = null;
    }

    [Test]
    public void Add_ShouldAddMeeting_WhenMeetingIsValid()
    {
      // Act
      _meetingRepository.Add(_testMeeting);

      // Assert
      var result = _meetingRepository.GetById(_testMeeting.Id);
      Assert.That(result, Is.EqualTo(_testMeeting));
    }

    [Test]
    public void Add_ShouldThrowArgumentNullException_WhenMeetingIsNull()
    {
      // Arrange
      Meeting meeting = null;

      // Act
      var result = () => _meetingRepository.Add(meeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Update_ShouldUpdateMeeting_WhenMeetingExists()
    {
      // Arrange
      _meetingRepository.Add(_testMeeting);
      var updatedMeeting = new Meeting
      {
        Id = _testMeeting.Id,
        Title = "Updated Meeting",
        StartTime = _testMeeting.StartTime,
        EndTime = _testMeeting.EndTime
      };

      // Act
      _meetingRepository.Update(updatedMeeting);

      // Assert
      var result = _meetingRepository.GetById(_testMeeting.Id);
      Assert.That(result.Title, Is.EqualTo("Updated Meeting"));
    }

    [Test]
    public void Update_ShouldThrowKeyNotFoundException_WhenMeetingDoesNotExist()
    {
      // Arrange
      var nonExistentMeeting = new Meeting { Id = 999 };

      // Act
      var result = () => _meetingRepository.Update(nonExistentMeeting);

      // Assert
      Assert.That(result, Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public void Remove_ShouldRemoveMeeting_WhenMeetingExists()
    {
      // Arrange
      _meetingRepository.Add(_testMeeting);

      // Act
      _meetingRepository.Remove(_testMeeting.Id);

      // Assert
      var result = () => _meetingRepository.GetById(_testMeeting.Id);
      Assert.That(result, Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public void Remove_ShouldThrowKeyNotFoundException_WhenMeetingDoesNotExist()
    {
      // Act
      var result = () => _meetingRepository.Remove(999);

      // Assert
      Assert.That(result, Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public void GetById_ShouldReturnMeeting_WhenMeetingExists()
    {
      // Arrange
      _meetingRepository.Add(_testMeeting);

      // Act
      var result = _meetingRepository.GetById(_testMeeting.Id);

      // Assert
      Assert.That(result, Is.EqualTo(_testMeeting));
    }

    [Test]
    public void GetById_ShouldThrowKeyNotFoundException_WhenMeetingDoesNotExist()
    {
      // Act
      var result = () => _meetingRepository.GetById(999);

      // Assert
      Assert.That(result, Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public void GetAll_ShouldReturnAllMeetingsInOrder()
    {
      // Arrange
      var meeting1 = new Meeting { StartTime = DateTime.Now.AddHours(3) };
      var meeting2 = new Meeting { StartTime = DateTime.Now.AddHours(1) };
      _meetingRepository.Add(meeting1);
      _meetingRepository.Add(meeting2);

      // Act
      var result = _meetingRepository.GetAll().ToList();

      // Assert
      Assert.That(result, Has.Count.EqualTo(2));
      Assert.That(result[0].StartTime, Is.LessThan(result[1].StartTime));
    }

    [Test]
    public void GetByDate_ShouldReturnMeetingsForSpecificDate()
    {
      // Arrange
      var today = DateTime.Today;
      var meeting1 = new Meeting { StartTime = today.AddHours(10) };
      var meeting2 = new Meeting { StartTime = today.AddDays(1) };
      _meetingRepository.Add(meeting1);
      _meetingRepository.Add(meeting2);

      // Act
      var result = _meetingRepository.GetByDate(today).ToList();

      // Assert
      Assert.That(result, Has.Count.EqualTo(1));
      Assert.That(result[0].StartTime.Date, Is.EqualTo(today));
    }

    [Test]
    public void GetUpcomingNotifications_ShouldReturnMeetingsNeedingNotification()
    {
      // Arrange
      var now = DateTime.Now;
      var meeting = new Meeting
      {
        StartTime = now.AddHours(1),
        NotificationTime = now.AddMinutes(30),
        NotificationShown = false
      };
      _meetingRepository.Add(meeting);

      // Act
      var result = _meetingRepository.GetUpcomingNotifications(now.AddMinutes(35)).ToList();

      // Assert
      Assert.That(result, Has.Count.EqualTo(1));
      Assert.That(result[0], Is.EqualTo(meeting));
    }

    [Test]
    public void GetUpcomingNotifications_ShouldReturnEmptyMeetingsList()
    {
      // Arrange
      var now = DateTime.Now;
      var meeting = new Meeting
      {
        StartTime = now.AddHours(1),
        NotificationTime = now.AddMinutes(30),
        NotificationShown = true
      };
      _meetingRepository.Add(meeting);

      // Act
      var result = _meetingRepository.GetUpcomingNotifications(now.AddMinutes(35)).ToList();

      // Assert
      Assert.That(result, Has.Count.EqualTo(0));
    }

    [Test]
    public void HasTimeConflict_ShouldReturnTrue_WhenMeetingsOverlap()
    {
      // Arrange
      var existingMeeting = new Meeting
      {
        StartTime = DateTime.Now.AddHours(1),
        EndTime = DateTime.Now.AddHours(2)
      };
      _meetingRepository.Add(existingMeeting);
      var newMeeting = new Meeting
      {
        StartTime = DateTime.Now.AddHours(1.5),
        EndTime = DateTime.Now.AddHours(2.5)
      };

      // Act
      var result = _meetingRepository.HasTimeConflict(newMeeting);

      // Assert
      Assert.That(result, Is.True);
    }

    [Test]
    public void HasTimeConflict_ShouldReturnFalse_WhenNoOverlap()
    {
      // Arrange
      var existingMeeting = new Meeting
      {
        StartTime = DateTime.Now.AddHours(1),
        EndTime = DateTime.Now.AddHours(2)
      };
      _meetingRepository.Add(existingMeeting);
      var newMeeting = new Meeting
      {
        StartTime = DateTime.Now.AddHours(3),
        EndTime = DateTime.Now.AddHours(4)
      };

      // Act
      var result = _meetingRepository.HasTimeConflict(newMeeting);

      // Assert
      Assert.That(result, Is.False);
    }

    [Test]
    public void HasTimeConflict_ShouldReturnFalse_WhenComparingWithSameMeeting()
    {
      // Arrange
      var meeting = new Meeting
      {
        Id = 1,
        StartTime = DateTime.Now.AddHours(1),
        EndTime = DateTime.Now.AddHours(2)
      };
      _meetingRepository.Add(meeting);

      // Act
      var result = _meetingRepository.HasTimeConflict(meeting);

      //Assert
      Assert.That(result, Is.False);
    }
  }
}