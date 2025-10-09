using System;
using Beverages;
using Condiments;

class Program
{
    static void Main()
    {
        IBeverage beverage = new Latte( Size.Double );
        beverage = new Cinnamon( beverage );
        beverage = new Lemon( beverage, 2 );
        beverage = new IceCubes( beverage, 2, IceCubeType.Dry );
        beverage = new ChocolateBar( beverage, 3 );
        beverage = new Liqueur( beverage, LiqueurType.Chocolate );

        Console.WriteLine( $"{beverage.GetDescription()} costs {beverage.GetCost()}" );

        IBeverage tea = new Tea( TeaType.Oolong );
        tea = new Cream( tea );
        tea = new Liqueur( tea, LiqueurType.Nut );
        Console.WriteLine( $"{tea.GetDescription()} costs {tea.GetCost()}" );
    }
}
