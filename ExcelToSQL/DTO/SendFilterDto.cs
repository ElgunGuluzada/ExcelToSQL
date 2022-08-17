using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExcelToSQL.DTO
{
    public class SendFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AccepttorEmail { get; set; }
        [Required]
        //[EnumDataType(typeof(SendType))]
        public SendType SendType { get; set; }

    }
    public class SendFilterDtoValidator: AbstractValidator<SendFilterDto>
    {
        public SendFilterDtoValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Cann't Be Empty");
            RuleFor(x => x.EndDate).NotEmpty().WithMessage("Cann't Be Empty");
            RuleFor(x => x.AccepttorEmail).NotEmpty().WithMessage("Email Address is Required").EmailAddress().WithMessage("A valid Email is Required");
            RuleFor(x => x).Custom((x, context) =>
            {
                string[] arr = x.AccepttorEmail.Split("@");
                if (arr[1].Trim().ToLower() != "code.edu.az")
                {
                    context.AddFailure("AcceptorEmail", "Only Domain Name <code.edu.az>");
                }
            });
            RuleFor(x => x).Custom((x, context) =>
              {
                  double time = (x.EndDate - x.StartDate).TotalMilliseconds;
                  if (time<0)
                  {
                      context.AddFailure("EndData", "Wrong Date");
                  }
              });

        }
        
    }

    public enum SendType
    {
        Segment = 1,
        Country,
        Product,
        Discount
    }
}
