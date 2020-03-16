﻿using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data.SqlClient;

namespace Logic.AppServices
{
    public class GetStudentsQuery : IQuery<List<StudentDto>>
    {
        public GetStudentsQuery(string enrolled, int? number)
        {
            EnrolledIn = enrolled;
            NumberOfCourses = number;
        }

        public string EnrolledIn { get; }
        public int? NumberOfCourses { get; }

        public class GetStudentsQueryHandler : IQueryHandler<GetStudentsQuery, List<StudentDto>>
        {
            private readonly QueriesConnectionString _connectionString;

            public GetStudentsQueryHandler(QueriesConnectionString connectionString)
            {
                _connectionString = connectionString;
            }

            public List<StudentDto> Handle(GetStudentsQuery query)
            {
                string sql = @"
                        SELECT s.StudentID, s.Name, s.Email,
	                        s.FirstCourseName Course1, s.FirstCourseCredits Course1Credits, s.FirstCourseGrade Course1Grade,
	                        s.SecondCourseName Course2, s.SecondCourseCredits Course2Credits, s.SecondCourseGrade Course2Grade
                        FROM dbo.Student s
                        WHERE (s.FirstCourseName = @Course
		                        OR s.SecondCourseName = @Course
		                        OR @Course IS NULL)
	                        AND (s.NumberOfEnrollments = @Number
		                        OR @Number IS NULL)
                        ORDER BY s.StudentID ASC";
                using (SqlConnection connection = new SqlConnection(_connectionString.Value))
                {
                    var students = connection
                        .Query<StudentDto>(sql, new
                        {
                            Course = query.EnrolledIn,
                            Number = query.NumberOfCourses
                        })
                        .ToList();
                    return students;
                }

            }

        }

    }
}
