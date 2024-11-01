// Добавляем необходимые библиотеки для работы с MySQL и сокетами
using MySql.Data.MySqlClient;
using System;
using System.Net.Sockets;

class Program
{
    // Объявляем переменную, чтобы контролировать завершение программы
    static int number = 0;

    static void Main(string[] args)
    {
        // Настройки подключения к серверу MySQL
        string server = "localhost";
        string port = "3310";
        string database = "University";
        string user = "root";
        string password = "KC49-MF31L";
        string connectionString = $"Server={server};Port={port};Database={database};User ID={user};Password={password};";

        // Проверка доступности сервера MySQL по указанному адресу и порту
        if (!IsServerAvailable(server, int.Parse(port)))
        {
            Console.WriteLine($"Сервер MySQL на {server}:{port} недоступен. Проверьте настройки.");
            return; // Если сервер недоступен, программа завершает выполнение
        }

        // Создание подключения к базе данных
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                // Открытие соединения с базой данных
                connection.Open();
                Console.WriteLine($"Соединение с базой данных '{database}' на сервере '{server}' установлено.");

                // Отключение проверок внешних ключей перед внесением изменений в базу
                SetForeignKeyChecks(connection, false);

                // Создание и очистка таблиц перед вставкой данных
                CreateTable(connection);
                ClearTable(connection);
                AddSampleData(connection);

                CreateTableTeachers(connection);
                ClearTableTeachers(connection);
                AddSampleTeachersData(connection);

                CreateTableCourses(connection);
                ClearTableCourses(connection);
                AddSampleCoursesData(connection);

                DropTableExams(connection);
                CreateTableExams(connection);
                ClearTableExams(connection);
                AddSampleExamsData(connection);

                CreateTableGrades(connection);
                ClearTableGrades(connection);
                AddSampleGradesData(connection);

                // Включение проверок внешних ключей обратно
                SetForeignKeyChecks(connection, true);

                // Основное меню программы
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Здравствуйте, это база данных университета.");
                while (number == 0) // Бесконечный цикл для работы с базой данных, пока не выбрана команда выхода
                {
                    ActionMenu(connection);
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений, возникающих при подключении и работе с базой данных
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    // Метод для отображения меню действий
    static void ActionMenu(MySqlConnection connection)
    {
        Console.WriteLine("Выберите пункт:");
        Console.WriteLine("1. Добавление нового студента, преподавателя, курса, экзамена и оценки.");
        Console.WriteLine("2. Изменение информации о студентах, преподавателях и курсах. При выборе введите категорию.");
        Console.WriteLine("3. Удаление студентов, преподавателей, курсов и экзаменов. При выборе введите категорию.");
        Console.WriteLine("4. Получение списка студентов по факультету.");
        Console.WriteLine("5. Получение списка курсов, читаемых определенным преподавателем.");
        Console.WriteLine("6. Получение списка студентов, зачисленных на конкретный курс.");
        Console.WriteLine("7. Получение оценок студентов по определенному курсу.");
        Console.WriteLine("8. Средний балл студента по определенному курсу.");
        Console.WriteLine("9. Средний балл студента по всем курсам.");
        Console.WriteLine("10. Средний балл по факультету.");
        Console.WriteLine("11. Перезапуск базы данных.");
        Console.WriteLine("0. Выход");

        int command;
        // Проверяем, является ли введенная строка числом, и если нет, выводим сообщение об ошибке
        while (!int.TryParse(Console.ReadLine(), out command) || command < 0 || command > 11)
        {
            Console.WriteLine("Некорректный ввод команды. Пожалуйста, введите число от 0 до 11.");
        }

        // Выполнение выбранного действия
        ActionMake(connection, command);
    }

    // Метод для выполнения команды из меню
    static void ActionMake(MySqlConnection connection, int command)
    {
        switch (command)
        {
            case 1:
            case 2:
            case 3:
                Console.WriteLine("Введите категорию. (Students, Teachers, Courses, Exams, Grades)");
                string category = Console.ReadLine();
                bool flag = true;

                if (category == "Students")
                {
                    // Логика для добавления, обновления или удаления студента
                    string Name;
                    do
                    {
                        Console.WriteLine("Введите имя.");
                        Name = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Name));

                    string Surname;
                    do
                    {
                        Console.WriteLine("Введите фамилию.");
                        Surname = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Surname));

                    string Department;
                    do
                    {
                        Console.WriteLine("Введите факультет.");
                        Department = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Department));

                    DateTime birthday;
                    while (true)
                    {
                        Console.WriteLine("Введите Дату рождения (дд.мм.гггг)");
                        if (DateTime.TryParse(Console.ReadLine(), out birthday))
                            break;
                        Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                    }

                    if (command == 1)
                    {
                        InsertUser(connection, Name, Surname, Department, birthday, true);
                    }
                    else if (command == 2)
                    {
                        Console.WriteLine("Введите номер(ID) студента.");
                        int StudentID;
                        while (!int.TryParse(Console.ReadLine(), out StudentID) || StudentID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        UpdateUser(connection, StudentID, Name, Surname, Department, birthday);
                    }
                    else if (command == 3)
                    {
                        Console.WriteLine("Введите номер(ID) студента.");
                        int StudentID;
                        while (!int.TryParse(Console.ReadLine(), out StudentID) || StudentID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        DeleteUser(connection, StudentID);
                    }
                    GetUsers(connection);
                }
                else if (category == "Teachers")
                {
                    // Логика для добавления, обновления или удаления преподавателя
                    string Name;
                    do
                    {
                        Console.WriteLine("Введите имя.");
                        Name = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Name));

                    string Surname;
                    do
                    {
                        Console.WriteLine("Введите фамилию.");
                        Surname = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Surname));

                    string Department;
                    do
                    {
                        Console.WriteLine("Введите факультет.");
                        Department = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Department));

                    if (command == 1)
                    {
                        InsertTeacher(connection, Name, Surname, Department, true);
                    }
                    else if (command == 2)
                    {
                        Console.WriteLine("Введите номер(ID) учителя.");
                        int TeacherID;
                        while (!int.TryParse(Console.ReadLine(), out TeacherID) || TeacherID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        UpdateTeachers(connection, TeacherID, Name, Surname, Department);
                    }
                    else if (command == 3)
                    {
                        Console.WriteLine("Введите номер(ID) учителя.");
                        int TeacherID;
                        while (!int.TryParse(Console.ReadLine(), out TeacherID) || TeacherID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        DeleteTeacher(connection, TeacherID);
                    }
                    GetTeachers(connection);
                }
                else if (category == "Courses")
                {
                    // Логика для добавления, обновления или удаления курса
                    string Title;
                    do
                    {
                        Console.WriteLine("Введите название курса.");
                        Title = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Title));

                    string Description;
                    do
                    {
                        Console.WriteLine("Введите описание курса.");
                        Description = Console.ReadLine();
                    } while (string.IsNullOrEmpty(Description));

                    Console.WriteLine("Введите ID учителя.");
                    int TeacherID;
                    while (!int.TryParse(Console.ReadLine(), out TeacherID) || TeacherID <= 0)
                    {
                        Console.WriteLine("Некорректный ввод. Введите положительное число.");
                    }

                    if (command == 1)
                    {
                        InsertCourse(connection, Title, Description, TeacherID, true);
                    }
                    else if (command == 2)
                    {
                        Console.WriteLine("Введите номер(ID) курса.");
                        int CourseiD;
                        while (!int.TryParse(Console.ReadLine(), out CourseiD) || CourseiD <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        UpdateCourse(connection, CourseiD, Title, Description, TeacherID);
                    }
                    else if (command == 3)
                    {
                        Console.WriteLine("Введите номер(ID) курса.");
                        int CourseID2;
                        while (!int.TryParse(Console.ReadLine(), out CourseID2) || CourseID2 <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        DeleteCourse(connection, CourseID2);
                    }
                    GetCourses(connection);
                }
                else if (category == "Exams")
                {
                    // Логика для добавления, обновления или удаления экзамена
                    DateTime examDate;
                    while (true)
                    {
                        Console.WriteLine("Введите день экзамена (дд.мм.гггг)");
                        if (DateTime.TryParse(Console.ReadLine(), out examDate))
                            break;
                        Console.WriteLine("Некорректный ввод. Попробуйте снова.");
                    }

                    Console.WriteLine("Введите ID курса.");
                    int courseId;
                    while (!int.TryParse(Console.ReadLine(), out courseId) || courseId <= 0)
                    {
                        Console.WriteLine("Некорректный ввод. Введите положительное число.");
                    }

                    Console.WriteLine("Введите максимальный балл (0-1000).");
                    decimal maxScore;
                    while (!decimal.TryParse(Console.ReadLine(), out maxScore) || maxScore < 0 || maxScore > 1000)
                    {
                        Console.WriteLine("Некорректный ввод. Максимальный балл должен быть в диапазоне от 0 до 1000.");
                    }

                    if (command == 1)
                    {
                        InsertExam(connection, examDate, courseId, maxScore, true);
                    }
                    else if (command == 2)
                    {
                        Console.WriteLine("Введите номер(ID) экзамена.");
                        int ExamID;
                        while (!int.TryParse(Console.ReadLine(), out ExamID) || ExamID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        UpdateExam(connection, ExamID, examDate, courseId, maxScore);
                    }
                    else if (command == 3)
                    {
                        Console.WriteLine("Введите номер(ID) экзамена.");
                        int ExamID;
                        while (!int.TryParse(Console.ReadLine(), out ExamID) || ExamID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        DeleteExam(connection, ExamID);
                    }
                    GetExams(connection);
                }
                else if (category == "Grades")
                {
                    // Логика для добавления, обновления или удаления оценки
                    int studentId;
                    Console.Write("Введите ID студента.");
                    while (!int.TryParse(Console.ReadLine(), out studentId) || studentId <= 0)
                    {
                        Console.WriteLine("Некорректный ввод. Введите положительное число.");
                    }

                    int examId;
                    Console.Write("Введите ID экзамена.");
                    while (!int.TryParse(Console.ReadLine(), out examId) || examId <= 0)
                    {
                        Console.WriteLine("Некорректный ввод. Введите положительное число.");
                    }

                    decimal score;
                    Console.Write("Введите оценку (0-100).");
                    while (!decimal.TryParse(Console.ReadLine(), out score) || score < 0 || score > 100)
                    {
                        Console.WriteLine("Некорректный ввод. Оценка должна быть в диапазоне от 0 до 100.");
                    }

                    if (command == 1)
                    {
                        InsertGrade(connection, studentId, examId, score, true);
                    }
                    else if (command == 2)
                    {
                        Console.WriteLine("Введите номер(ID) оценки.");
                        int GradeID;
                        while (!int.TryParse(Console.ReadLine(), out GradeID) || GradeID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        UpdateGrade(connection, GradeID, studentId, examId, score);
                    }
                    else if (command == 3)
                    {
                        Console.WriteLine("Введите номер(ID) оценки.");
                        int GradeID;
                        while (!int.TryParse(Console.ReadLine(), out GradeID) || GradeID <= 0)
                        {
                            Console.WriteLine("Некорректный ввод. Введите положительное число.");
                        }
                        DeleteGrade(connection, GradeID);
                    }
                    GetGrades(connection);
                }
                else
                {
                    Console.WriteLine("Некорректная категория.");
                }
                break;

            case 4:
                Console.WriteLine("Введите факультет.");
                string department;
                do
                {
                    department = Console.ReadLine();
                    if (string.IsNullOrEmpty(department))
                    {
                        Console.WriteLine("Пустая строка. Введите факультет снова.");
                    }
                } while (string.IsNullOrEmpty(department));
                GetStudentsByDepartment(connection, department);
                break;
            case 5:
                Console.WriteLine("Введите имя учителя.");
                string NameTeacher;
                do
                {
                    NameTeacher = Console.ReadLine();
                    if (string.IsNullOrEmpty(NameTeacher))
                    {
                        Console.WriteLine("Пустая строка. Введите имя снова.");
                    }
                } while (string.IsNullOrEmpty(NameTeacher));
                GetCoursesByTeacherName(connection, NameTeacher);
                break;
            case 6:
            case 7:
                int courseID;
                Console.Write("Введите ID курса.");
                while (!int.TryParse(Console.ReadLine(), out courseID) || courseID <= 0)
                {
                    Console.WriteLine("Некорректный ввод. Введите положительное число.");
                }
                if (command == 6)
                {
                    StudentNameCourseID(connection, courseID);
                }
                if (command == 7)
                {
                    GetStudentGradesByCourse(connection, courseID);
                }
                break;
            case 8:
            case 9:
                int CourseID;
                Console.Write("Введите ID курса.");
                while (!int.TryParse(Console.ReadLine(), out CourseID) || CourseID <= 0)
                {
                    Console.WriteLine("Некорректный ввод. Введите положительное число.");
                }
                Console.Write("Введите ID студента.");
                int studentID;
                while (!int.TryParse(Console.ReadLine(), out studentID) || studentID <= 0)
                {
                    Console.WriteLine("Некорректный ввод. Введите положительное число.");
                }
                if (command == 8)
                {
                    AverageFromCourses(connection, CourseID, studentID);
                }
                if (command == 9)
                {
                    AverageFromAll(connection, studentID);
                }
                break;
            case 10:
                Console.WriteLine("Введите факультет.");
                string departmentst;
                do
                {
                    departmentst = Console.ReadLine();
                    if (string.IsNullOrEmpty(departmentst))
                    {
                        Console.WriteLine("Пустая строка. Введите факультет снова.");
                    }
                } while (string.IsNullOrEmpty(departmentst));
                AverageScoreByDepartment(connection, departmentst);
                break;
            case 11:
                Console.WriteLine("Выполняется перезапуск базы данных");
                CreateTable(connection);
                ClearTable(connection);
                AddSampleData(connection);

                CreateTableTeachers(connection);
                ClearTableTeachers(connection);
                AddSampleTeachersData(connection);

                CreateTableCourses(connection);
                ClearTableCourses(connection);
                AddSampleCoursesData(connection);

                CreateTableExams(connection);
                ClearTableExams(connection);
                AddSampleExamsData(connection);

                CreateTableGrades(connection);
                ClearTableGrades(connection);
                AddSampleGradesData(connection);
                break;
            case 0:
                CloseOver(); // Увеличиваем переменную для выхода из цикла
                connection.Close(); // Закрываем соединение
                break;
            default:
                Console.WriteLine("Некорректный пункт.");
                break;
        }
    }

    // Метод для выхода из программы
    static void CloseOver()
    {
        number += 1;
    }

    // Метод для проверки доступности сервера MySQL
    static bool IsServerAvailable(string server, int port)
    {
        try
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(server, port);
                return true; // Если подключение удалось, возвращаем true
            }
        }
        catch (SocketException)
        {
            return false; // Если возникло исключение, сервер недоступен
        }
    }

    // Отключение или включение проверки внешних ключей
    static void SetForeignKeyChecks(MySqlConnection connection, bool enable)
    {
        using (var command = new MySqlCommand(enable ? "SET FOREIGN_KEY_CHECKS=1;" : "SET FOREIGN_KEY_CHECKS=0;", connection))
        {
            command.ExecuteNonQuery();
        }
    }

    static void CreateTable(MySqlConnection connection)
    {
        using (var command = new MySqlCommand(@"
            CREATE TABLE IF NOT EXISTS Students (
                ID INT AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(100) NOT NULL,
                Surname VARCHAR(100) NOT NULL,
                Department VARCHAR(100) NOT NULL,
                DateOfBirth DATE NOT NULL,
                UNIQUE(Name, Surname)
            );", connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Students' успешно создана.");
        }
    }
    static void ClearTable(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("DELETE FROM Students;", connection))
        {
            command.ExecuteNonQuery();
        }
        using (var command = new MySqlCommand("ALTER TABLE Students AUTO_INCREMENT = 1;", connection))
        {
            command.ExecuteNonQuery();
        }
    }
    static void AddSampleData(MySqlConnection connection)
    {
        var students = new (string Name, string Surname, string Department, DateTime BirthDate)[]
        {
            ("Михаил", "Антонов", "Физика", new DateTime(2001, 4, 25)),
            ("Антон", "Иванов", "Компьютерные науки", new DateTime(1998, 9, 30)),
            ("Максим", "Кузмичёв", "Экономика", new DateTime(2003, 10, 28)),
            ("Алексей", "Соколов", "Психология", new DateTime(2000, 6, 11)),
            ("Алиса", "Смирнова", "Физика", new DateTime(2004, 2, 6)),
            ("Мария", "Морозова", "Журналистика", new DateTime(2004, 2, 6)),
            ("Николай", "Петров", "Компьютерные науки", new DateTime(2004, 2, 6)),
            ("Игорь", "Захаров", "Экономика", new DateTime(2004, 2, 6))
        };
        foreach (var student in students)
        {
            InsertUser(connection, student.Name, student.Surname, student.Department, student.BirthDate, false);
        }
    }
    static bool UserExists(MySqlConnection connection, string name, string surname)
    {
        using (var command = new MySqlCommand("SELECT COUNT(*) FROM Students WHERE Name = @name AND Surname = @surname", connection))
        {
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }

    static void InsertUser(MySqlConnection connection, string name, string surname, string department, DateTime dateOfBirth, bool write)
    {
        if (UserExists(connection, name, surname))
        {
            Console.WriteLine($"Пользователь {name} {surname} уже существует.");
            return;
        }

        using (var command = new MySqlCommand("INSERT INTO Students (Name, Surname, Department, DateOfBirth) VALUES (@name, @surname, @department, @dateOfBirth)", connection))
        {
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth);
            command.ExecuteNonQuery();
            if (write)
            {
                Console.WriteLine($"Пользователь {name} {surname} добавлен.");
            }
        }
    }
    static void GetUsers(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("SELECT * FROM Students", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["ID"]}, Имя: {reader["Name"]}, Фамилия: {reader["Surname"]}, Кафедра: {reader["Department"]}, Дата рождения: {reader["DateOfBirth"]}");
            }
        }
    }
    static void UpdateUser(MySqlConnection connection, int studentId, string newName, string newSurname, string newDepartment, DateTime newBirthday)
    {
        string query = @"
        UPDATE Students 
        SET Name = @newName, Surname = @newSurname, Department = @newDepartment, Birthday = @newBirthday 
        WHERE ID = @studentId";

        static void UpdateUser(MySqlConnection connection, int studentId, string newName, string newSurname, string newDepartment, DateTime newBirthday)
        {
            string checkQuery = "SELECT COUNT(*) FROM Students WHERE ID = @studentId";

            using (var checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@studentId", studentId);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count == 0)
                {
                    Console.WriteLine($"Пользователь с ID '{studentId}' не найден.");
                    return;
                }
            }
            string query = @"
            UPDATE Students 
            SET Name = @newName, Surname = @newSurname, Department = @newDepartment, Birthday = @newBirthday 
            WHERE ID = @studentId";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@newName", newName);
                command.Parameters.AddWithValue("@newSurname", newSurname);
                command.Parameters.AddWithValue("@newDepartment", newDepartment);
                command.Parameters.AddWithValue("@newBirthday", newBirthday);
                command.Parameters.AddWithValue("@studentId", studentId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Информация о пользователе с ID '{studentId}' успешно обновлена на '{newName} {newSurname}' в отделе '{newDepartment}' с днём рождения '{newBirthday.ToShortDateString()}'.");
                }
                else
                {
                    Console.WriteLine($"Информация о пользователе с ID '{studentId}' не была обновлена.");
                }
            }
        }

    }
    static void DeleteUser(MySqlConnection connection, int studentId)
    {
        string query = "DELETE FROM Students WHERE ID = @studentId";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@studentId", studentId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine($"Студент с ID '{studentId}' успешно удалён.");
            }
            else
            {
                Console.WriteLine($"Студент с ID '{studentId}' не найден.");
            }
        }
    }
    static void CreateTableTeachers(MySqlConnection connection)
    {
        using (var command = new MySqlCommand(@"
            CREATE TABLE IF NOT EXISTS Teachers (
                ID INT AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(100) NOT NULL,
                Surname VARCHAR(100) NOT NULL,
                Department VARCHAR(100) NOT NULL
            );", connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Teachers' успешно создана.");
        }
    }

    static void ClearTableTeachers(MySqlConnection connection)
    {
        {
            using (var command = new MySqlCommand("DELETE FROM Teachers;", connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new MySqlCommand("ALTER TABLE Teachers AUTO_INCREMENT = 1;", connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
    static void AddSampleTeachersData(MySqlConnection connection)
    {
        var teachers = new (string Name, string Surname, string Department)[]
        {
            ("Сергей", "Маслов", "Физика"),
            ("Елена", "Сидорова", "Компьютерные науки"),
            ("Александр", "Николаев", "Экономика"),
            ("Татьяна", "Смирнова", "Психология"),
            ("Ольга", "Кузнецова", "Физика"),
            ("Иван", "Беляев", "Журналистика"),
            ("Анна", "Павлова", "Компьютерные науки"),
            ("Дмитрий", "Захаров", "Экономика")
        };
        foreach (var teacher in teachers)
        {
            InsertTeacher(connection, teacher.Name, teacher.Surname, teacher.Department, false);
        }
    }
    static void InsertTeacher(MySqlConnection connection, string name, string surname, string department, bool write)
    {
        if (TeacherExists(connection, name, surname))
        {
            Console.WriteLine($"Преподаватель {name} {surname} уже существует.");
            return;
        }

        using (var command = new MySqlCommand("INSERT INTO Teachers (Name, Surname, Department) VALUES (@name, @surname, @department)", connection))
        {
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@department", department);
            command.ExecuteNonQuery();
            if (write)
            {
                Console.WriteLine($"Преподаватель {name} {surname} добавлен.");
            }
        }
    }

    static bool TeacherExists(MySqlConnection connection, string name, string surname)
    {
        using (var command = new MySqlCommand("SELECT COUNT(*) FROM Teachers WHERE Name = @name AND Surname = @surname", connection))
        {
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
    }

    static void GetTeachers(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("SELECT * FROM Teachers", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["ID"]}, Имя: {reader["Name"]}, Фамилия: {reader["Surname"]}, Кафедра: {reader["Department"]}");
            }
        }
    }

    static void UpdateTeachers(MySqlConnection connection, int teacherId, string newName, string newSurname, string newDepartment)
    {
        string checkQuery = "SELECT COUNT(*) FROM Teachers WHERE ID = @teacherId";

        using (var checkCommand = new MySqlCommand(checkQuery, connection))
        {
            checkCommand.Parameters.AddWithValue("@teacherId", teacherId);
            int count = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (count == 0)
            {
                Console.WriteLine($"Преподаватель с ID '{teacherId}' не найден.");
                return;
            }
        }

        string query = @"
    UPDATE Teachers 
    SET Name = @newName, Surname = @newSurname, Department = @newDepartment 
    WHERE ID = @teacherId";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@newName", newName);
            command.Parameters.AddWithValue("@newSurname", newSurname);
            command.Parameters.AddWithValue("@newDepartment", newDepartment);
            command.Parameters.AddWithValue("@teacherId", teacherId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine($"Информация о преподавателе с ID '{teacherId}' успешно обновлена на '{newName} {newSurname}' в кафедре '{newDepartment}'.");
            }
            else
            {
                Console.WriteLine($"Информация о преподавателе с ID '{teacherId}' не была обновлена.");
            }
        }
    }



    static void DeleteTeacher(MySqlConnection connection, int teacherId)
    {
        string query = "DELETE FROM Teachers WHERE ID = @teacherId";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@teacherId", teacherId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine($"Преподаватель с ID '{teacherId}' успешно удалён.");
            }
            else
            {
                Console.WriteLine($"Преподаватель с этим ID не найден.");
            }
        }
    }


    static void CreateTableCourses(MySqlConnection connection)
    {
        using (var command = new MySqlCommand(@"
            CREATE TABLE IF NOT EXISTS Courses (
                ID INT AUTO_INCREMENT PRIMARY KEY,
                Title VARCHAR(255) NOT NULL,
                Description TEXT NOT NULL,
                TeacherID INT,
                FOREIGN KEY (TeacherID) REFERENCES Teachers(ID) ON DELETE CASCADE
            );", connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Courses' успешно создана.");
        }
    }

    static void ClearTableCourses(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("DELETE FROM Courses;", connection))
        {
            command.ExecuteNonQuery();
        }
        using (var command = new MySqlCommand("ALTER TABLE Courses AUTO_INCREMENT = 1;", connection))
        {
            command.ExecuteNonQuery();
        }
    }

    static void AddSampleCoursesData(MySqlConnection connection)
    {
        var courses = new (string Title, string Description, int TeacherID)[]
        {
            ("Новая физика", "Углубление по физике.", 1),
            ("Изучение Python", "Изучение основ программирования на языке Python.", 2),
            ("Экономика сегодня", "Курс, посвященный экономике в текущем времени.", 3),
            ("Психология человека", "Изучение основных понятий психологии.", 4),
            ("Простые механизмы", "Создание и изучение простых механизмов.", 1),
            ("Журналистские исследования", "Основы журналистики и исследовательской работы.", 6),
            ("Алгоритмы и структуры данных", "Изучение алгоритмов и структур данных в программировании.", 2),
            ("Управление менеджментом", "Основы управления менеджментом и управление расходами.", 3),
            ("Физика для инженеров", "Курс по физике, ориентированный на инженеров.", 1),
            ("Основы финансового анализа", "Изучение основ финансового анализа и отчетности.", 5)
        };

        foreach (var course in courses)
        {
            InsertCourse(connection, course.Title, course.Description, course.TeacherID, false);
        }
    }

    static void InsertCourse(MySqlConnection connection, string title, string description, int teacherId, bool write)
    {
        using (var command = new MySqlCommand("SELECT COUNT(*) FROM Teachers WHERE ID = @teacherId", connection))
        {
            command.Parameters.AddWithValue("@teacherId", teacherId);
            var count = Convert.ToInt32(command.ExecuteScalar());
            if (count == 0)
            {
                Console.WriteLine($"Преподаватель с ID '{teacherId}' не существует. Курс '{title}' не добавлен.");
                return;
            }
        }

        string query = "INSERT INTO Courses (Title, Description, TeacherID) VALUES (@Title, @Description, @TeacherID)";
        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Title", title);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@TeacherID", teacherId);
            command.ExecuteNonQuery();
            if (write)
            {
                Console.WriteLine($"Курс '{title}' успешно добавлен.");
            }
        }
    }

    static void GetCourses(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("SELECT * FROM Courses", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["ID"]}, Название: {reader["Title"]}, Описание: {reader["Description"]}, ID преподавателя: {reader["TeacherID"]}");
            }
        }
    }

    static void UpdateCourse(MySqlConnection connection, int courseId, string newTitle, string newDescription, int newTeacherId)
    {
        using (var command = new MySqlCommand("UPDATE Courses SET Title = @title, Description = @description, TeacherID = @teacherId WHERE ID = @courseId", connection))
        {
            command.Parameters.AddWithValue("@title", newTitle);
            command.Parameters.AddWithValue("@description", newDescription);
            command.Parameters.AddWithValue("@teacherId", newTeacherId);
            command.Parameters.AddWithValue("@courseId", courseId);

            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine($"Курс с ID '{courseId}' успешно обновлен.");
            }
            else
            {
                Console.WriteLine($"Курс с этим ID  не найден.");
            }
        }
    }

    static void DeleteCourse(MySqlConnection connection, int courseId)
    {
        using (var deleteExamsCmd = new MySqlCommand("DELETE FROM Exams WHERE CourseID = @courseId", connection))
        {
            deleteExamsCmd.Parameters.AddWithValue("@courseId", courseId);
            deleteExamsCmd.ExecuteNonQuery();
        }

        using (var deleteCourseCmd = new MySqlCommand("DELETE FROM Courses WHERE ID = @courseId", connection))
        {
            int rowsAffected = deleteCourseCmd.ExecuteNonQuery();

            deleteCourseCmd.Parameters.AddWithValue("@courseId", courseId);
            if (rowsAffected > 0)
            {
                Console.WriteLine($"Курс с ID '{courseId}' и все связанные экзамены удалены.");
            }
            else
            {
                Console.WriteLine($"Курс с этим ID не найден.");
            }
        }
    }
    static void DropTableExams(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("DROP TABLE IF EXISTS Exams;", connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Exams' удалена.");
        }
    }

    static void CreateTableExams(MySqlConnection connection)
    {
        using (var command = new MySqlCommand(@"
        CREATE TABLE IF NOT EXISTS Exams (
            ID INT AUTO_INCREMENT PRIMARY KEY,
            CourseID INT,
            ExamDate DATE,
            MaxScore DECIMAL(5, 2),
            FOREIGN KEY (CourseID) REFERENCES Courses(ID) ON DELETE CASCADE
        );", connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Exams' успешно создана.");
        }
    }

    static void ClearTableExams(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("DELETE FROM Exams;", connection))
        {
            command.ExecuteNonQuery();
        }
        using (var command = new MySqlCommand("ALTER TABLE Exams AUTO_INCREMENT = 1;", connection))
        {
            command.ExecuteNonQuery();
        }
    }

    static void AddSampleExamsData(MySqlConnection connection)
    {
        var exams = new (int CourseID, DateTime ExamDate, decimal MaxScore)[]
        {
        (1, new DateTime(2024, 5, 15), 120.00m),
        (2, new DateTime(2024, 6, 10), 90.30m),
        (3, new DateTime(2024, 7, 20), 150.45m),
        (4, new DateTime(2024, 2, 23), 170.80m),
        };

        foreach (var exam in exams)
        {
            InsertExam(connection, exam.ExamDate, exam.CourseID, exam.MaxScore, false);
        }
    }

    static void InsertExam(MySqlConnection connection, DateTime examDate, int courseId, decimal maxscore, bool write)
    {
        CreateTableExams(connection);

        using (var transaction = connection.BeginTransaction())
        {
            using (var command = new MySqlCommand("SELECT COUNT(*) FROM Courses WHERE ID = @courseId", connection, transaction))
            {
                command.Parameters.AddWithValue("@courseId", courseId);
                var count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    Console.WriteLine($"Курс с ID '{courseId}' не существует. Экзамен не добавлен.");
                    transaction.Rollback();
                    return;
                }
            }

            string query = "INSERT INTO Exams (CourseID, ExamDate, MaxScore) VALUES (@CourseID, @ExamDate, @MaxScore)";
            using (var command = new MySqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ExamDate", examDate);
                command.Parameters.AddWithValue("@CourseID", courseId);
                command.Parameters.AddWithValue("@MaxScore", maxscore);
                command.ExecuteNonQuery();
                transaction.Commit();

                if (write)
                {
                    Console.WriteLine($"Экзамен для курса ID '{courseId}' успешно добавлен.");
                }
            }
        }
    }


    static void GetExams(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("SELECT * FROM Exams", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["ID"]}, ID курса: {reader["CourseID"]}, Дата экзамена: {reader["ExamDate"]}, Максимальный балл {reader["MaxScore"]} ");
            }
        }
    }

    static void UpdateExam(MySqlConnection connection, int examId, DateTime newExamDate, int newCourseId, decimal newMaxScore)
    {
        using (var command = new MySqlCommand("UPDATE Exams SET CourseID = @courseId, ExamDate = @examDate, MaxScore = @MaxScore WHERE ID = @examId", connection))
        {
            command.Parameters.AddWithValue("@examDate", newExamDate);
            command.Parameters.AddWithValue("@courseId", newCourseId);
            command.Parameters.AddWithValue("@MaxScore", newMaxScore);
            command.Parameters.AddWithValue("@examId", examId);
            command.ExecuteNonQuery();
            Console.WriteLine($"Экзамен с ID '{examId}' успешно обновлён.");
        }
    }

    static void DeleteExam(MySqlConnection connection, int examId)
    {
        using (var command = new MySqlCommand("DELETE FROM Exams WHERE ID = @examId", connection))
        {
            command.Parameters.AddWithValue("@examId", examId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine($"Экзамен с ID '{examId}' успешно удалён.");
            }
            else
            {
                Console.WriteLine($"Экзамен с ID '{examId}' не найден.");
            }
        }
    }
    static void CreateTableGrades(MySqlConnection connection)
    {
        using (var command = new MySqlCommand(@"
            CREATE TABLE IF NOT EXISTS Grades (
                ID INT AUTO_INCREMENT PRIMARY KEY,
                StudentID INT,
                ExamID INT,
                Score DECIMAL(5, 2),
                FOREIGN KEY (StudentID) REFERENCES Students(ID) ON DELETE CASCADE,
                FOREIGN KEY (ExamID) REFERENCES Exams(ID) ON DELETE CASCADE
            );", connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Таблица 'Grades' успешно создана.");
        }
    }

    static void ClearTableGrades(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("DELETE FROM Grades;", connection))
        {
            command.ExecuteNonQuery();
        }
        using (var command = new MySqlCommand("ALTER TABLE Grades AUTO_INCREMENT = 1;", connection))
        {
            command.ExecuteNonQuery();
        }
    }


    static void AddSampleGradesData(MySqlConnection connection)
    {
        var grades = new (int StudentID, int ExamID, decimal Score)[]
        {
            (1, 1, 85.50m),
            (2, 2, 90.00m),
            (3, 3, 78.00m),
            (4, 4, 92.00m),
            (1, 2, 88.25m),
            (2, 3, 79.50m),
            (3, 4, 84.00m),
            (4, 1, 91.00m)
        };

        foreach (var grade in grades)
        {
            InsertGrade(connection, grade.StudentID, grade.ExamID, grade.Score, false);
        }
    }
    static void InsertGrade(MySqlConnection connection, int studentId, int examId, decimal score, bool write)
    {
        using (var command = new MySqlCommand("SELECT COUNT(*) FROM Students WHERE ID = @studentId", connection))
        {
            command.Parameters.AddWithValue("@studentId", studentId);
            var studentCount = Convert.ToInt32(command.ExecuteScalar());
            if (studentCount == 0)
            {
                Console.WriteLine($"Студент с этим ID  не существует. Оценка не добавлена.");
                return;
            }
        }

        using (var command = new MySqlCommand("SELECT COUNT(*) FROM Exams WHERE ID = @examId", connection))
        {
            command.Parameters.AddWithValue("@examId", examId);
            var examCount = Convert.ToInt32(command.ExecuteScalar());
            if (examCount == 0)
            {
                Console.WriteLine($"Экзамен с введёным ID не существует. Оценка не добавлена.");
                return;
            }
        }

        int maxScore;
        using (var command = new MySqlCommand("SELECT MaxScore FROM Exams WHERE ID = @examId", connection))
        {
            command.Parameters.AddWithValue("@examId", examId);
            maxScore = Convert.ToInt32(command.ExecuteScalar());
        }

        if (score < 0 || score > maxScore)
        {
            Console.WriteLine($"Ошибка: Оценка должна быть в диапазоне Экзамена.");
            return;
        }

        string query = "INSERT INTO Grades (StudentID, ExamID, Score) VALUES (@StudentID, @ExamID, @Score)";
        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@StudentID", studentId);
            command.Parameters.AddWithValue("@ExamID", examId);
            command.Parameters.AddWithValue("@Score", score);
            command.ExecuteNonQuery();
            if (write)
            {
                Console.WriteLine($"Оценка '{score}' добавлена для студента ID '{studentId}' на экзамене ID '{examId}'.");
            }
        }
    }
    static void GetGrades(MySqlConnection connection)
    {
        using (var command = new MySqlCommand("SELECT * FROM Grades", connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["ID"]}, Студент ID: {reader["StudentID"]}, Экзамен ID: {reader["ExamID"]}, Оценка: {reader["Score"]}");
            }
        }
    }

    static void UpdateGrade(MySqlConnection connection, int gradeId, int studentId, int examId, decimal newScore)
    {
        string query = "UPDATE Grades SET Score = @score WHERE ID = @gradeId AND StudentID = @studentId AND ExamID = @examId";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@score", newScore);
            command.Parameters.AddWithValue("@gradeId", gradeId);
            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@examId", examId);
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine($"Оценка с ID '{gradeId}' для студента с ID '{studentId}' на экзамене с ID '{examId}' успешно обновлена на {newScore}.");
            }
            else
            {
                Console.WriteLine($"Оценка с ID '{gradeId}' для студента с ID '{studentId}' и экзамена с ID '{examId}' не найдена.");
            }
        }
    }
    static void DeleteGrade(MySqlConnection connection, int gradeId)
    {
        using (var command = new MySqlCommand("DELETE FROM Grades WHERE ID = @gradeId", connection))
        {
            command.Parameters.AddWithValue("@gradeId", gradeId);
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine($"Оценка с ID '{gradeId}' успешно удалена.");
            }
            else
            {
                Console.WriteLine($"Оценка с ID '{gradeId}' не найдена.");
            }
        }
    }
    static void GetStudentsByDepartment(MySqlConnection connection, string department)
    {
        string query = "SELECT * FROM Students WHERE Department = @department";
        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@department", department);

            using (var StudentData = command.ExecuteReader())
            {
                if (StudentData.HasRows)
                {
                    Console.WriteLine($"Список студентов факультета '{department}':");
                    while (StudentData.Read())
                    {
                        int id = StudentData.GetInt32(("ID"));
                        string name = StudentData.GetString("Name");
                        string surname = StudentData.GetString("Surname");
                        Console.WriteLine($"ID: {id}, Имя: {name}, Фамилия: {surname}");
                    }
                }
                else
                {
                    Console.WriteLine($"Нет студентов на факультете '{department}'.");
                }
            }
        }
    }
    static void GetCoursesByTeacherName(MySqlConnection connection, string teacherName)
    {
        string query = @"
        SELECT c.Title 
        FROM Courses c
        JOIN Teachers t ON c.TeacherID = t.ID
        WHERE t.Name = @teacherName";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@teacherName", teacherName);

            try
            {
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine($"Преподаватель с именем '{teacherName}' не найден.");
                        return;
                    }

                    Console.WriteLine("Список курсов:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"- {reader["Title"]}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
    static void StudentNameCourseID(MySqlConnection connection, int teacherId)
    {
        string query = @"
        SELECT s.Name, s.Surname
        FROM Students s
        JOIN Teachers t ON s.Department = t.Department
        JOIN Courses c ON t.ID = c.TeacherID
        WHERE t.ID = @teacherId;";
        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@teacherId", teacherId);
            using (var TeacherFind = command.ExecuteReader())
            {
                if (TeacherFind.Read())
                {
                    string name = TeacherFind.GetString("Name");
                    string surname = TeacherFind.GetString("Surname");
                    Console.WriteLine(name + " " + surname);
                }
                else
                {
                    Console.WriteLine($"Ученики не найдены");
                }
            }
        }
    }
    static void GetStudentGradesByCourse(MySqlConnection connection, int courseId)
    {
        string query = @"
    SELECT s.Name, s.Surname, g.Score
    FROM Students s
    JOIN Grades g ON s.ID = g.StudentID
    JOIN Exams e ON g.ExamID = e.ID
    JOIN Courses c ON e.CourseID = c.ID
    WHERE c.ID = @courseId";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@courseId", courseId);

            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine($"Нет оценок для курса с этим ID.");
                    return;
                }

                Console.WriteLine($"Оценки студентов для курса с ID '{courseId}':");
                while (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    string surname = reader["Surname"].ToString();
                    int score = Convert.ToInt32(reader["Score"]);

                    Console.WriteLine($"Студент: {name} {surname}, Оценка: {score}");
                }
            }
        }
    }

    static void AverageFromCourses(MySqlConnection connection, int courseId, int studentId)
    {
        string query = @"
    SELECT AVG(g.Score) AS AverageGrade
    FROM Grades g
    JOIN Exams e ON g.ExamID = e.ID
    JOIN Students s ON s.ID = g.StudentID
    WHERE e.CourseID = @courseId AND g.StudentID = @studentId";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@courseId", courseId);
            command.Parameters.AddWithValue("@studentId", studentId);

            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Данные не найдены.");
                    return;
                }

                while (reader.Read())
                {
                    double averageScore = reader["AverageGrade"] != DBNull.Value
                                          ? Convert.ToDouble(reader["AverageGrade"])
                                          : 0.0;
                    Console.WriteLine($"Средняя оценка студента с ID {studentId} по курсу с ID {courseId}: {averageScore}");
                }
            }
        }
    }

    static void AverageFromAll(MySqlConnection connection, int studentId)
    {
        string query = @"
    SELECT AVG(g.Score) AS AverageGrade
    FROM Grades g
    JOIN Students s ON s.ID = g.StudentID
    WHERE g.StudentID = @studentId";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@studentId", studentId);

            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Нет данных для данного студента.");
                    return;
                }

                while (reader.Read())
                {
                    double averageScore = reader["AverageGrade"] != DBNull.Value
                                          ? Convert.ToDouble(reader["AverageGrade"])
                                          : 0.0;
                    Console.WriteLine($"Средняя оценка студента с ID {studentId}: {averageScore}");
                }
            }
        }
    }

    static void AverageScoreByDepartment(MySqlConnection connection, string department)
    {
        string query = @"
        SELECT AVG(g.Score) AS AverageGrade
        FROM Grades g
        JOIN Students s ON s.ID = g.StudentID
        JOIN Exams e ON g.ExamID = e.ID
        JOIN Courses c ON e.CourseID = c.ID
        WHERE s.Department = @department
        GROUP BY s.Department;";

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@department", department);

            using (var ScoreDepartment = command.ExecuteReader())
            {
                if (!ScoreDepartment.HasRows)
                {
                    Console.WriteLine($"Не найдено данных для факультета.");
                    return;
                }

                while (ScoreDepartment.Read())
                {
                    decimal averageScore = ScoreDepartment.GetDecimal("AverageGrade");
                    Console.WriteLine($"Средняя оценка для факультета '{department}': {averageScore}");
                }
            }
        }
    }
}
