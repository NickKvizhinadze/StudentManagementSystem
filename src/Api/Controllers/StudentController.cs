using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/students")]
    public sealed class StudentController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly StudentRepository _studentRepository;
        private readonly CourseRepository _courseRepository;
        private readonly Messages _messages;

        public StudentController(UnitOfWork unitOfWork, Messages messages)
        {
            _unitOfWork = unitOfWork;
            _studentRepository = new StudentRepository(unitOfWork);
            _courseRepository = new CourseRepository(unitOfWork);
            _messages = messages;
        }

        [HttpGet]
        public IActionResult GetList(string enrolled, int? number)
        {
            var query = new GetStudentsQuery(enrolled, number);
            var dtos = _messages.Dispatch(query);
            return Ok(dtos);
        }

        [HttpPost]
        public IActionResult Register([FromBody] NewStudentDto dto)
        {
            var result = _messages.Dispatch(new RegisterStudentCommand(
                     dto.Name,
                     dto.Email,
                     dto.Course1,
                     dto.Course1Grade,
                     dto.Course2,
                     dto.Course2Grade
                     )
                 );
            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public IActionResult UnRegister(long id)
        {
            var result = _messages.Dispatch(new UnRegisterStudentCommand(id));
            return FromResult(result);
        }

        [HttpPost("{id}/enrollments")]
        public IActionResult Enroll(long id, [FromBody] StudentEnrollmentDto dto)
        {
            var result = _messages.Dispatch(new EnrollStudentCommand(id, dto.Course, dto.Grade));
            return FromResult(result);
        }

        [HttpPut("{id}/enrollments/{enrollmentNumber}")]
        public IActionResult Transfer(long id, int enrollmentNumber, [FromBody] StudentTransferDto dto)
        {
            var result = _messages.Dispatch(new TransferStudentCommand(id, enrollmentNumber, dto.Course, dto.Grade));
            return FromResult(result);
        }

        [HttpPost("{id}/enrollments/{enrollmentNumber}/deletion")]
        public IActionResult Disenrollment(long id, int enrollmentNumber, [FromBody] StudentDisenrollmentDto dto)
        {
            var result = _messages.Dispatch(new DisEnrollStudentCommand(id, enrollmentNumber, dto.Comment));
            return FromResult(result);
        }

        [HttpPut("{id}")]
        public IActionResult EditPersonalInfo(long id, [FromBody] StudentPersonalInfoDto dto)
        {
            var command = new EditPersonalInfoCommand(id, dto.Name, dto.Email);

            var result = _messages.Dispatch(command);
            return FromResult(result);
        }

    }
}
