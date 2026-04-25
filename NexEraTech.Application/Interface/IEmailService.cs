using NexEraTech.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexEraTech.Application.Interface
{
    public interface IEmailService
    {
        Task<bool> SendAppointmentConfirmationAsync(Users user, string appointmentDetails);
    }
}
