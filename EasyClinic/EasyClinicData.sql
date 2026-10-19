-- Specializations
INSERT INTO Specializations (Name) VALUES
('باطنة'),
('مخ وأعصاب'),
('أسنان'),
('تحاليل');

-- MedicalServices
INSERT INTO MedicalServices (Name, Description, Price, DurationInMinutes) VALUES
('كشف', N'كشف عام لدى الطبيب', 150.00, 20),
('استشارة', N'استشارة طبية متابعة', 100.00, 15),
('تحليل دم', N'تحليل دم شامل (CBC)', 200.00, 10);

-- Doctors (SpecializationId بيرجع لترتيب الإدخال فوق: 1=باطنة, 2=مخ وأعصاب, 3=أسنان, 4=تحاليل)
INSERT INTO Doctors (FullName, PhoneNumber, Email, SpecializationId) VALUES
(N'د. أحمد سعيد', '01012345678', 'ahmed.saeed@easyclinic.com', 1),
(N'د. سارة محمود', '01098765432', 'sara.mahmoud@easyclinic.com', 2),
(N'د. محمد علي', '01055512345', 'mohamed.ali@easyclinic.com', 4);

-- DoctorServices (ربط كل دكتور بالخدمات اللي بيقدمها)
-- د. أحمد (باطنة، Id=1) بيقدم كشف واستشارة
INSERT INTO DoctorServices (DoctorId, MedicalServiceId) VALUES
(1, 1), -- كشف
(1, 2), -- استشارة
(2, 1), -- د. سارة بتقدم كشف
(2, 2), -- د. سارة بتقدم استشارة
(3, 3); -- د. محمد (تحاليل) بيقدم تحليل دم بس

-- AppointmentStatuses
INSERT INTO AppointmentStatuses (Name, Description) VALUES
(N'Pending', N'الحجز في انتظار التأكيد'),
(N'Confirmed', N'تم تأكيد الحجز'),
(N'Completed', N'تم إتمام الكشف/الخدمة'),
(N'Cancelled', N'تم إلغاء الحجز');

-- Patients
INSERT INTO Patients (Name, DateOfBirth, Gender, PhoneNumber, MedicalHistory) VALUES
(N'كريم حسن', '1990-05-14', 0, '01234567890', N'لا يوجد أمراض مزمنة'),
(N'منى فاروق', '1985-11-22', 1, '01198765432', N'حساسية من البنسلين');

-- Appointments (استخدمنا DoctorServiceId المتولدة فوق)
INSERT INTO Appointments (PatientId, DoctorServiceId, AppointmentStatusId, AppointmentDate, TotalPrice, Notes) VALUES
(1, 1, 2, '2026-09-15 10:00:00 +02:00', 150.00, N'أول زيارة'),
(2, 3, 1, '2026-09-16 12:30:00 +02:00', 150.00, NULL);