using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxTraining
{
    public class EmployeePayment
    {
        public string Name { get; private set; }
        public double Amount { get; private set; }

        public EmployeePayment(string name, double amount)
        {
            this.Name = name;
            this.Amount = amount;
        }
    }

    // TODO #1 Take a gander
    //         Reactive Extensions use the observer pattern
    //         C# is a pain in the ass for creating observable objects
    //         Rx provides the ability to make this a lot easier. I present the Subject!
    public class EmployeeManger
    {
        private readonly ISubject<EmployeePayment> employeePaid;

        public EmployeeManger()
        {
            // A subject implements IObservable so you can avoid creating 
            // custom implementations for observable objects
            this.employeePaid = new Subject<EmployeePayment>();
        }

        public IObservable<EmployeePayment> EmployeePaid { get { return this.employeePaid; } }

        public void PayEmployee(EmployeePayment payment)
        {
            // Call OnNext to publish the value to the subscribers
            this.employeePaid.OnNext(payment);
        }

        public void PayEmployee(string name, double amount)
        {
            this.PayEmployee(new EmployeePayment(name, amount));
        }

        public IObservable<EmployeePayment> SpecificEmployeePaid(string name)
        {
            return this.EmployeePaid;
        }

        public IObservable<EmployeePayment> PotentialAnnualSalary()
        {
            return this.EmployeePaid;
        }
    }
}
