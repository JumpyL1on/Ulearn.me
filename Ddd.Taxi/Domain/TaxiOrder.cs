using Ddd.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ddd.Taxi.Domain
{
    public class DriversRepository
    {
        public void FillDriverToOrder(int driverId, TaxiOrder order)
        {
            if (driverId == 15)
            {
                var car = new Car("Baklazhan", "Lada sedan", "A123BT 66");
                var driver = new Driver(driverId, new PersonName("Drive", "Driverson"), car);
                order.GetType().GetProperty("Driver").SetValue(order, driver);
            }
            else throw new Exception("Unknown driver id " + driverId);
        }
    }

    public class Driver : Entity<int>
    {
        public PersonName DriverName { get; }
        public Car Car { get; }

        public Driver(int id, PersonName driverName, Car car) : base(id)
        {
            DriverName = driverName;
            Car = car;
        }
    }

    public class Car : ValueType<Car>
    {
        public string CarColor { get; }
        public string CarModel { get; }
        public string CarPlateNumber { get; }

        public Car(string carColor, string carModel, string carPlateNumber)
        {
            CarColor = carColor;
            CarModel = carModel;
            CarPlateNumber = carPlateNumber;
        }
    }

    public class TaxiApi : ITaxiApi<TaxiOrder>
    {
        private readonly DriversRepository _driversRepo;
        private readonly Func<DateTime> _currentTime;
        private int _idCounter;

        public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
        {
            _driversRepo = driversRepo;
            _currentTime = currentTime;
        }

        public TaxiOrder CreateOrderWithoutDestination(
            string firstName,
            string lastName,
            string street,
            string building)
        {
            var clientName = new PersonName(firstName, lastName);
            var start = new Address(street, building);
            return TaxiOrder.CreateOrderWithoutDestination(_idCounter++, _currentTime, clientName, start);
        }

        public void UpdateDestination(TaxiOrder order, string street, string building)
        {
            order.UpdateDestination(new Address(street, building));
        }

        public void AssignDriver(TaxiOrder order, int driverId)
        {
            order.AssignDriver(order, driverId, _currentTime, _driversRepo);
        }

        public void UnassignDriver(TaxiOrder order)
        {
            order.UnassignDriver();
        }

        public string GetDriverFullInfo(TaxiOrder order)
        {
            return order.GetDriverFullInfo();
        }

        public string GetShortOrderInfo(TaxiOrder order)
        {
            return order.GetShortOrderInfo();
        }

        public void Cancel(TaxiOrder order)
        {
            order.Cancel(_currentTime);
        }

        public void StartRide(TaxiOrder order)
        {
            order.StartRide(_currentTime);
        }

        public void FinishRide(TaxiOrder order)
        {
            order.FinishRide(_currentTime);
        }
    }

    public class TaxiOrder : Entity<int>
    {
        public PersonName ClientName { get; private set; }
        public Address Start { get; private set; }
        public Address Destination { get; private set; }
        public Driver Driver { get; private set; }
        public TaxiOrderStatus Status { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime DriverAssignmentTime { get; private set; }
        public DateTime CancelTime { get; private set; }
        public DateTime StartRideTime { get; private set; }
        public DateTime FinishRideTime { get; private set; }

        private TaxiOrder(int id) : base(id)
        {
        }

        public static TaxiOrder CreateOrderWithoutDestination(
            int id,
            Func<DateTime> currentTime,
            PersonName clientName,
            Address start)
        {
            return new TaxiOrder(id)
            {
                ClientName = clientName,
                Start = start,
                Destination = new Address(null, null),
                Driver = new Driver(-1, new PersonName(null, null), new Car(null, null, null)),
                CreationTime = currentTime(),
                Status = TaxiOrderStatus.WaitingForDriver
            };
        }

        public void UpdateDestination(Address destination)
        {
            var rightStatuses = new HashSet<TaxiOrderStatus>()
            {
                TaxiOrderStatus.WaitingForDriver,
                TaxiOrderStatus.WaitingCarArrival
            };
            if (rightStatuses.Contains(Status))
                Destination = destination;
            else throw new InvalidOperationException(Status.ToString());
        }

        public void AssignDriver(
            TaxiOrder order,
            int driverId,
            Func<DateTime> currentTime,
            DriversRepository driversRepo)
        {
            if (Status == TaxiOrderStatus.WaitingForDriver)
            {
                driversRepo.FillDriverToOrder(driverId, order);
                order.Status = TaxiOrderStatus.WaitingCarArrival;
                DriverAssignmentTime = currentTime();
            }
            else throw new InvalidOperationException(Status.ToString());
        }

        public void UnassignDriver()
        {
            var rightStatuses = new HashSet<TaxiOrderStatus>()
            {
                TaxiOrderStatus.Canceled,
                TaxiOrderStatus.Finished,
                TaxiOrderStatus.WaitingCarArrival
            };
            if (rightStatuses.Contains(Status))
            {
                Driver = new Driver(-1, new PersonName(null, null), new Car(null, null, null));
                Status = TaxiOrderStatus.WaitingForDriver;
            }
            else throw new InvalidOperationException(Status.ToString());
        }

        public string GetDriverFullInfo()
        {
            if (Status == TaxiOrderStatus.WaitingForDriver) return null;
            return string.Join(" ",
                "Id: " + Driver.Id,
                "DriverName: " + FormatName(Driver.DriverName.FirstName, Driver.DriverName.LastName),
                "Color: " + Driver.Car.CarColor,
                "CarModel: " + Driver.Car.CarModel,
                "PlateNumber: " + Driver.Car.CarPlateNumber);
        }

        public string GetShortOrderInfo()
        {
            return string.Join(" ",
                "OrderId: " + Id,
                "Status: " + Status,
                "Client: " + FormatName(ClientName.FirstName, ClientName.LastName),
                "Driver: " + FormatName(Driver.DriverName.FirstName, Driver.DriverName.LastName),
                "From: " + FormatAddress(Start.Street, Start.Building),
                "To: " + FormatAddress(Destination.Street, Destination.Building),
                "LastProgressTime: " + GetLastProgressTime()
                    .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }

        public void Cancel(Func<DateTime> currentTime)
        {
            if (Status == TaxiOrderStatus.WaitingCarArrival || Status == TaxiOrderStatus.WaitingForDriver)
            {
                Status = TaxiOrderStatus.Canceled;
                CancelTime = currentTime();
            }
            else throw new InvalidOperationException(Status.ToString());
        }

        public void StartRide(Func<DateTime> currentTime)
        {
            if (Status == TaxiOrderStatus.WaitingCarArrival)
            {
                Status = TaxiOrderStatus.InProgress;
                StartRideTime = currentTime();
            }
            else throw new InvalidOperationException(Status.ToString());
        }

        public void FinishRide(Func<DateTime> currentTime)
        {
            var rightStatuses = new HashSet<TaxiOrderStatus>()
            {
                TaxiOrderStatus.InProgress,
                TaxiOrderStatus.Canceled
            };
            if (rightStatuses.Contains(Status))
            {
                Status = TaxiOrderStatus.Finished;
                FinishRideTime = currentTime();
            }
            else throw new InvalidOperationException(Status.ToString());
        }

        private DateTime GetLastProgressTime()
        {
            return Status switch
            {
                TaxiOrderStatus.WaitingForDriver => CreationTime,
                TaxiOrderStatus.WaitingCarArrival => DriverAssignmentTime,
                TaxiOrderStatus.InProgress => StartRideTime,
                TaxiOrderStatus.Finished => FinishRideTime,
                TaxiOrderStatus.Canceled => CancelTime,
                _ => throw new NotSupportedException(Status.ToString()),
            };
        }

        private static string FormatName(string firstName, string lastName)
        {
            return string.Join(" ", new[] {firstName, lastName}.Where(n => n != null));
        }

        private static string FormatAddress(string street, string building)
        {
            return string.Join(" ", new[] {street, building}.Where(n => n != null));
        }
    }
}