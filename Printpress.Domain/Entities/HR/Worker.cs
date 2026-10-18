using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class Worker : Entity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public SalaryType SalaryType { get; set; }
        public decimal? MonthlySalary { get; set; }
        public decimal? DailySalary { get; set; }
        public decimal UnpaidAdvanceAmount { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }


        public virtual ICollection<WorkerSalaryTransaction> SalaryTransactions { get; set; } = [];
        public virtual ICollection<ItemServiceExecution> WorkerProductions { get; set; } = [];
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = [];
        public virtual ICollection<WorkerAttendance> Attendance { get; private set; }

        public WorkerSalaryTransaction AddTransaction(
            Guid id,
            Guid payrollPeriodId,
            SalaryTransactionType transactionType,
            decimal amount,
            DateTime transactionDate,
            string? note)
        {
            if(transactionType == SalaryTransactionType.SalaryAdvance)
            {
                UnpaidAdvanceAmount += amount;
            }

            if(transactionType == SalaryTransactionType.SalaryAdvancePayment)
            {
                UnpaidAdvanceAmount -= amount;
            }

            var transaction = new WorkerSalaryTransaction
            {
                Id = id,
                WorkerId = Id,
                PayrollPeriodId = payrollPeriodId,
                TransactionType = transactionType,
                Amount = amount,
                TransactionDate = transactionDate,
                Note = note
            };

            SalaryTransactions.Add(transaction);

            return transaction;
        }

        public bool CanChangeSalaryType(bool workHasSalaryTransactions)
        {
            return workHasSalaryTransactions ? false : true;
        }

        public WorkerAttendance CheckIn(DateTime time)
        {
            EnsureAttendanceLoaded();

            if (time == default)
                throw new BusinessExceptions(LocalizationKeys.HR.TimeRequired);

            if (Attendance.Any(a => !a.IsVoided && a.CheckOutAt is null))
                throw new BusinessExceptions(LocalizationKeys.HR.AlreadyCheckedIn);

            EnsureArrivalIsFree(time);

            var stretch = new WorkerAttendance(Id, time);
            Attendance.Add(stretch);
            return stretch;
        }

        public WorkerAttendance CheckOut(DateTime time)
        {
            EnsureAttendanceLoaded();

            if (time == default)
                throw new BusinessExceptions(LocalizationKeys.HR.TimeRequired);

            var open = Attendance.SingleOrDefault(a => !a.IsVoided && a.CheckOutAt is null)
                ?? throw new BusinessExceptions(LocalizationKeys.HR.NotCheckedIn);

            EnsureNoOverlap(open.CheckInAt, time, open);
            open.Close(time);
            return open;
        }

        public WorkerAttendance AddClosed(DateTime checkInAt, DateTime checkOutAt)
        {
            EnsureAttendanceLoaded();

            var stretch = new WorkerAttendance(Id, checkInAt);
            stretch.Close(checkOutAt);
            EnsureNoOverlap(stretch.CheckInAt, stretch.CheckOutAt);
            Attendance.Add(stretch);
            return stretch;
        }

        private void EnsureAttendanceLoaded()
        {
            if (Attendance is null)
                throw new InvalidOperationException("Worker.Attendance was not loaded.");
        }

        private void EnsureArrivalIsFree(DateTime time)
        {
            var insideAnother = Attendance.Any(a =>
                !a.IsVoided
                && a.CheckInAt <= time
                && time < (a.CheckOutAt ?? DateTime.MaxValue));

            if (insideAnother)
                throw new BusinessExceptions(LocalizationKeys.HR.StretchOverlaps);
        }

        private void EnsureNoOverlap(DateTime checkInAt, DateTime? checkOutAt, WorkerAttendance self = null)
        {
            var end = checkOutAt ?? DateTime.MaxValue;
            var overlaps = Attendance.Any(a =>
                !a.IsVoided
                && (self is null || !ReferenceEquals(a, self))
                && IntervalsOverlap(checkInAt, end, a.CheckInAt, a.CheckOutAt ?? DateTime.MaxValue));

            if (overlaps)
                throw new BusinessExceptions(LocalizationKeys.HR.StretchOverlaps);
        }

        private static bool IntervalsOverlap(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
            => aStart < bEnd && bStart < aEnd;
    }
}
