using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace RxTraining
{
    public interface IEmail
    {
        void Send(string message);
    }

    public class EmployeePaidConsumer : IDisposable
    {
        private readonly IEmail email;
        private readonly IDisposable subscription;

        // TODO #3 Observables are first class!
        //         Observables can be passed around in parameters, making them first class objects.
        //         Events cannot.
        //         Imagine employeePaid was an event, how would this class look?
        //         What if the handler was a lamda or expression? How would you detach the handler?
        //         What if an error occured during an employee getting paid?
        //         This is where observables come in handy
        public EmployeePaidConsumer(IObservable<EmployeePayment> employeePaid, IEmail email)
        {
            this.email = email;
            this.subscription = employeePaid
                .Subscribe(
                    this.EmployeePaidHandler, 
                    ex => this.email.Send(string.Format("There was an error processing payment: {0}", ex.Message))
                );
        }

        public void Dispose()
        {
            this.subscription.Dispose();
        }

        private void EmployeePaidHandler(EmployeePayment payment)
        {
            Console.WriteLine("{0} got paid £{1}", payment.Name, payment.Amount);
            this.email.Send(string.Format("Congrats {0}, you just got paid £{1}!", payment.Name, payment.Amount));
        }
    }
}
