-- drop TABLE Enrollments;
-- drop TABLE CourseInstances;
-- drop TABLE Courses;
-- drop TABLE Students;

CREATE TABLE IF NOT EXISTS Courses (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    DurationHours INTEGER NOT NULL,
	CreatedDate TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS Students (
    Id SERIAL PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS CourseInstances (
    Id SERIAL PRIMARY KEY,
    CourseId INT NOT NULL,
    StartDate TIMESTAMPTZ NOT NULL,
    EndDate TIMESTAMPTZ NOT NULL,
    StudentsAmount INT NOT NULL,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id),
    CHECK (EndDate > StartDate)
);

CREATE TABLE IF NOT EXISTS Enrollments (
    Id SERIAL PRIMARY KEY,
    StudentId INT NOT NULL,
    CourseInstanceId INT NOT NULL,
    EnrollmentDate TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
	IsCompleted BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (StudentId) REFERENCES Students(Id),
    FOREIGN KEY (CourseInstanceId) REFERENCES CourseInstances(Id),
    UNIQUE (StudentId, CourseInstanceId)
);

CREATE OR REPLACE FUNCTION check_course_enrollment()
RETURNS TRIGGER AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM Enrollments e
        JOIN CourseInstances ci ON e.CourseInstanceId = ci.Id
        WHERE e.StudentId = NEW.StudentId
        AND ci.CourseId = (SELECT CourseId FROM CourseInstances WHERE Id = NEW.CourseInstanceId)
    ) THEN
        RAISE EXCEPTION 'Студент уже записан на этот курс';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_prevent_duplicate_course
BEFORE INSERT ON Enrollments
FOR EACH ROW EXECUTE FUNCTION check_course_enrollment();
