using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

[DataContract]
class Car : ICloneable
{
    static readonly string[] randomModels = { "Audi", "BMW", "Opel", "Lada", "Peugeot", "Nissan", "Tesla" };
    static readonly string[] randomFeatures = { "Air Back", "ABS", "Пiдiгрiв сидiння", "GPS", "Сигналiзацiя", "ESP" };

    [DataMember]
    public string Mark { get; set; }
    [DataMember]
    public int Power { get; set; }
    [DataMember]
    public double Engine { get; set; }
    [DataMember]
    public double CountOfFuel { get; set; }
    [DataMember]
    public double Consumption { get; set; }
    [DataMember]
    public string[] Features { get; set; }

    public Car()
    {

    }

    public Car(string mark, int power, double engine, double countOfFuel, double consumption, string[] features)
    {
        Mark = mark;
        Power = power;
        Engine = engine;
        CountOfFuel = countOfFuel;
        Consumption = consumption;
        Features = features;
    }

    public static Car GenerateRandomCar()
    {
        Random random = new Random();
        return new Car
        {
            Mark = randomModels[random.Next(0, 7)],
            Power = random.Next(100, 400),
            Engine = Math.Round((random.NextDouble() + 1) * 2, 2),
            CountOfFuel = random.Next(50, 100),
            Consumption = random.Next(5, 20),
            Features = new string[]
            {
                randomFeatures[random.Next(0, 2)], randomFeatures[random.Next(3, 5)]
            }
        };
    }

    public static Car CreateCar()
    {
        Console.WriteLine("Введiть:");
        Console.WriteLine("Марку:");
        string mark = Console.ReadLine();
        Console.WriteLine("Потужнiсть в к.с.: ");
        int.TryParse(Console.ReadLine(), out int power);
        Console.WriteLine("Об'єм двигуна в л.:");
        double.TryParse(Console.ReadLine(), out double engine);
        Console.WriteLine("Кiлькiсть палива в баку:");
        double.TryParse(Console.ReadLine(), out double countOfFuel);
        Console.WriteLine("Витрата палива на 100км.:");
        double.TryParse(Console.ReadLine(), out double consumption);
        Console.WriteLine("Додатковi функцiї (через кому якщо їх декiлька):");
        string[] features = Console.ReadLine().Split(',');
        return new Car(mark, power, engine, countOfFuel, consumption, features);
    }

    public void ShowCarMenu()
    {
        bool flag = true;
        while (flag)
        {
            Console.WriteLine("Натиснiть:");
            Console.WriteLine("1 щоб вивести iнформацiю про авто");
            Console.WriteLine("2 щоб заправити автомобiль");
            Console.WriteLine("3 щоб проїхати вказану вiдстань");
            Console.WriteLine("4 щоб дiзнатись на яку вiдстань вистачить палива");
            Console.WriteLine("5 для виходу");
            int.TryParse(Console.ReadLine(), out int choose);
            switch (choose)
            {
                case 1: Console.WriteLine(ToString()); break;
                case 2: FillUp(); break;
                case 3: PassTheDistance(); break;
                case 4: GetRange(); break;
                case 5: flag = false; break;
            }
            Console.WriteLine();
        }
    }

    public override string ToString()
    {
        string features = string.Join(",", Features);
        return "*********************************\n\n" +
               $" Марка: {Mark}\n" +
               $" Потужнiсть: {Power}к.с.\n" +
               $" Двигун: {Engine}л.\n" +
               $" Кiлькiсть палива в баку: {CountOfFuel}л.\n" +
               $" Витрата палива на 100км.: {Consumption}л.\n" +
               $" Додатковi функцiї: {features}\n\n" +
                "*********************************";
    }

    void FillUp() //заправити
    {
        Console.WriteLine("Введiть кiлькiсть літрів пального:");
        int.TryParse(Console.ReadLine(), out int fuel);
        CountOfFuel += fuel;
        Console.WriteLine($"Автомобiль заправлений на {fuel}л.");
    }

    void PassTheDistance()
    {
        Console.WriteLine("Введiть вiдстань:");
        int.TryParse(Console.ReadLine(), out int kilometers);
        if (kilometers <= GetMaximumRange())
        {
            CountOfFuel -= kilometers / 100.0 * Consumption;
            Console.WriteLine($"Ви проїхали {kilometers}км.\nЗалишилось {CountOfFuel}л. палива");
        }
        else
        {
            Console.WriteLine("Не достатньо палива!");
        }
    }

    double GetMaximumRange()
    {
        return Math.Round((100.0 / Consumption * CountOfFuel), 2);
    }

    void GetRange()
    {
        double km = GetMaximumRange();
        Console.WriteLine($"Ви зможете ще проїхати: {km} км.");
    }

    public object Clone()
    {
        Car car = (Car)MemberwiseClone();
        car.Features = (string[])Features.Clone();
        return car;
    }

    public class CompareByModel : IComparer
    {
        public int Compare(object objectCar1, object objectCar2)
        {
            Car car1 = objectCar1 as Car;
            Car car2 = objectCar2 as Car;
            return car1.Mark.CompareTo(car2.Mark);
        }
    }

    public class CompareByPower : IComparer
    {
        public int Compare(object objectCar1, object objectCar2)
        {
            Car car1 = objectCar1 as Car;
            Car car2 = objectCar2 as Car;
            return car1.Power.CompareTo(car2.Power);
        }
    }
}

class Program
{
    static List<Car> listOfCars;
    static Action serealize = Serealize;
    static Action deserealize = Deserealize;

    static void Main(string[] args)
    {
        ShowMainMenu();
    }

    static void ShowMainMenu()
    {
        bool flag = true;
        while (flag)
        {
            Console.WriteLine("Натиснiть:\n" +
                "1 для додавання нового автомобiля\n" +
                "2 для роботи з автомобiлем\n" +
                "3 для видалення автомобiля\n" +
                "4 для виведення iнформацiї про усi автомобiлi\n" +
                "5 для пошуку за параметром\n" +
                "6 для сортування\n" +
                "7 для клонування\n" +
                "8 для сереалiзацiї\n" +
                "9 для десереалiзацiї\n" +
                "10 для виходу\n");

            int.TryParse(Console.ReadLine(), out int button);
            switch (button)
            {
                case 1: AddNewCar(); break;
                case 2: SelectCar(UseCar); break;
                case 3: SelectCar(DeleteCar); break;
                case 4: ShowCars(); break;
                case 5: SearchByParameter(); break;
                case 6: SortListOfCar(); break;
                case 7: SelectCar(CloneCar); break;
                case 8: Serealize(); break;
                case 9: Deserealize(); break;
                case 10: flag = false; break;
                default: Console.WriteLine("Помилково вказаний номер"); break;
            }
            Console.WriteLine();
        }
    }

    private static void Deserealize()
    {
        DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Car>));

        using (FileStream fs = new FileStream("car.json", FileMode.OpenOrCreate))
        {
            listOfCars = new List<Car>();
            listOfCars = (List<Car>)jsonFormatter.ReadObject(fs);
        }




        Console.WriteLine("Десереалiзацiя пройшла успiшно");
    }

    private static void Serealize()
    {
        DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Car>));
        try
        {
            using (FileStream fs = new FileStream("car.json", FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(fs, listOfCars);
            }
        }
        catch (Exception exeption)
        {
            Console.WriteLine($"Помилка - {exeption.Data}");
            return;
        }
        Console.WriteLine("Сереалiзацiя пройшла успiшно");
    }

    static void CloneCar(Car car)
    {
        Car newCar = (Car)car.Clone();
        listOfCars.Add(newCar);
    }

    static void SortListOfCar()
    {
        Console.WriteLine("Натиснiть:\n" +
                          "1 для сортування по марцi\n" +
                          "2 для сортування по потужностi");
        int.TryParse(Console.ReadLine(), out int userNumber);
        Car[] arrayOfCars = listOfCars.ToArray();
        if (userNumber == 1)
        {
            Array.Sort(arrayOfCars, new Car.CompareByModel());
        }
        else if (userNumber == 2)
        {
            Array.Sort(arrayOfCars, new Car.CompareByPower());
        }
        else
        {
            Console.WriteLine("Не коректний номер!");
            return;
        }
        listOfCars = new List<Car>(arrayOfCars);
    }

    static void SearchByParameter()
    {
        Console.WriteLine("\nНатиснiть:\n" +
            "1 для пошуку по марцi\n" +
            "2 для пошуку по потужностi\n" +
            "3 для пошуку по додатковим функцiям");
        int.TryParse(Console.ReadLine(), out int userNumber);
        if (userNumber <= 0 || userNumber > 3)
        {
            Console.WriteLine("Не коректно введенi даннi");
        }
        Console.WriteLine("Введiть даннi для пошуку:\n");
        string data = Console.ReadLine();
        SearchByParameter(userNumber, data);
    }

    static void SearchByParameter(int numberOfMenu, string data)
    {
        List<Car> newListOfCar = new List<Car>();
        switch (numberOfMenu)
        {
            case 1:
                string mark = data;
                foreach (var car in listOfCars)
                {
                    if (car.Mark == mark)
                    {
                        newListOfCar.Add(car);
                    }
                }
                break;

            case 2:
                double power = Convert.ToDouble(data);
                foreach (var car in listOfCars)
                {
                    if (car.Power == power)
                    {
                        newListOfCar.Add(car);
                    }
                }
                break;

            case 3:
                string newFeature = data;
                foreach (var car in listOfCars)
                {
                    foreach (var feature in car.Features)
                    {
                        if (newFeature == feature)
                        {
                            newListOfCar.Add(car);
                            break;
                        }
                    }
                }
                break;
        }
        ShowCars(false, newListOfCar);
    }

    static void UseCar(Car car)
    {
        car.ShowCarMenu();
    }

    static void DeleteCar(Car car)
    {
        listOfCars.Remove(car);
    }

    static void SelectCar(Action<Car> action)
    {
        if (listOfCars == null || listOfCars.Count < 1)
        {
            Console.WriteLine("\nУ вас немає нiодного авто, спершу додайте його");
            return;
        }
        ShowCars(true);
        Console.WriteLine("Введiть номер автомобiля який вам потрiбен:");
        int.TryParse(Console.ReadLine(), out int userNumber);
        if (userNumber > 0 && userNumber <= listOfCars.Count)
        {
            action(listOfCars[--userNumber]);
        }
        else
        {
            Console.WriteLine("Не коректний номер");
        }
    }

    static void AddNewCar()
    {
        if (listOfCars == null)
        {
            listOfCars = new List<Car>();
        }

        Console.WriteLine("\nНатиснiть\n" +
                          "1 для введення данних вручну\n" +
                          "2 для генерацiї автомобiля випадково\n");
        int.TryParse(Console.ReadLine(), out int userNumber);
        listOfCars.Add(userNumber == 1 ? Car.CreateCar() : Car.GenerateRandomCar());
    }

    static void ShowCars(bool withIndex = false, List<Car> inputListOfCars = null)
    {
        if (listOfCars == null || listOfCars.Count < 1)
        {
            Console.WriteLine("\nУ вас немає нiодного авто, спершу додайте його");
            return;
        }
        int index = 1;
        foreach (var car in listOfCars)
        {
            if (withIndex)
            {
                Console.WriteLine("*********************************\n" +
                                  $"\t\t{index++}");
            }
            if (inputListOfCars != null)
            {
                if (!inputListOfCars.Contains(car))
                {
                    return;
                }
            }
            Console.WriteLine(car);
        }
    }
}