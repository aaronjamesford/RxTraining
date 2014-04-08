using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using RxTraining;

namespace RxTrainingTests
{
    [TestFixture]
    public class SubjectTests
    {
        private IObserver<EmployeePayment> mockObserver;
        private EmployeeManger manager;

        [SetUp]
        public void Setup()
        {
            this.mockObserver = Substitute.For<IObserver<EmployeePayment>>();
            this.manager = new EmployeeManger();
        }

        [Test]
        public void EmployeePaidIsTriggeredWhenPayEmployeeInvoked()
        {
            using (this.manager.EmployeePaid.Subscribe(this.mockObserver))
            {
                this.manager.PayEmployee("aaron", 455.67);
                this.mockObserver.Received(1).OnNext(Arg.Any<EmployeePayment>());
            }
        }

        [Test]
        public void SpecificEmployeePaidIsNotTriggeredWhenNotSameName()
        {
            // TODO #2 Fix the implementation of SpecificEmployeePaid to make this test pass
            //         Hint: you can use Linq!
            using (this.manager.SpecificEmployeePaid("john").Subscribe(this.mockObserver))
            {
                this.manager.PayEmployee("aaron", 455.67);
                this.mockObserver.Received(0).OnNext(Arg.Any<EmployeePayment>());
            }
        }

        [Test]
        public void SpecificEmployeePaidTriggeredWhenIsPaid()
        {
            using (this.manager.SpecificEmployeePaid("aaron").Subscribe(this.mockObserver))
            {
                var payment = new EmployeePayment("aaron", 455.67);
                this.manager.PayEmployee(payment);
                this.mockObserver.Received(1).OnNext(payment);
            }
        }

        [Test]
        public void PotentialAnnualSalaryIsCorrectWhenPaymentIsMade()
        {
            // TODO #2.5 Fix the implementation of PotentialAnnualSalary to make this test pass
            using (this.manager.PotentialAnnualSalary().Subscribe(this.mockObserver))
            {
                this.manager.PayEmployee("aaron", 2);
                this.mockObserver.Received(1).OnNext(Arg.Is<EmployeePayment>(p => p.Amount == 24));
            }
        }
    }
}
