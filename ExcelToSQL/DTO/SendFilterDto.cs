using FluentValidation;
using System;

namespace ExcelToSQL.DTO
{
    public class SendFilterDto
    {
        public DateTime StartData { get; set; }
        public DateTime EndData { get; set; }
        public string AccepttorEmail { get; set; }

    }
    public class SendFilterDtoValidator: AbstractValidator<SendFilterDto>
    {
        public SendFilterDtoValidator()
        {
            RuleFor(x => x.StartData).NotEmpty().WithMessage("Cann't Be Empty");
            RuleFor(x => x.EndData).NotEmpty().WithMessage("Cann't Be Empty");
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
                  double time = (x.EndData - x.StartData).TotalMilliseconds;
                  if (time<0)
                  {
                      context.AddFailure("EndData", "Wrong Date");
                  }
              });

        }
        
    }

    public enum SendType
    {
        Segment= 1,
        Country,
        Product,
        Discount
    }
}
