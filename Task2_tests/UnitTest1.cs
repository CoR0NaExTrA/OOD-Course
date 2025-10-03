using System;
using System.Collections.Generic;
using NUnit.Framework;
using WeatherStation;
using WeatherStation.Models;
using WeatherStation.Core;

namespace WeatherStation.Tests;

// ��������������� �����������, ������� ���������� ����������� ����������
class RecordingObserver : IObserver<WeatherInfo>
{
    public readonly List<WeatherInfo> Received = new List<WeatherInfo>();
    public void Update( WeatherInfo data, IObservable<WeatherInfo> source )
    {
        Received.Add( data );
    }
}

// �����������, ������� ������� ��� ���� � ���� Update
class SelfRemovingObserver : IObserver<WeatherInfo>
{
    public readonly List<WeatherInfo> Received = new List<WeatherInfo>();
    public void Update( WeatherInfo data, IObservable<WeatherInfo> source )
    {
        Received.Add( data );
        // ������� ���� � ���� ���������, ��� ��������� ����������� �� ����� ������� �����������
        source.RemoveObserver( this );
    }
}

// ����������� ��� �������� ������� ����������� (���������� ����� � ������)
class OrderingObserver : IObserver<WeatherInfo>
{
    private readonly string _name;
    private readonly List<string> _orderList;
    public OrderingObserver( string name, List<string> orderList )
    {
        _name = name;
        _orderList = orderList;
    }
    public void Update( WeatherInfo data, IObservable<WeatherInfo> source )
    {
        _orderList.Add( _name );
    }
}

// �����������, ����������� ��� source � Update ������������� ���������� observable
class SourceAwareObserver<T> : IObserver<T>
{
    public readonly List<IObservable<T>> Sources = new List<IObservable<T>>();
    public void Update( T data, IObservable<T> source )
    {
        Sources.Add( source );
    }
}

[TestFixture]
public class WeatherStationTests
{
    [Test]
    public void Test_Observer_Removes_Self_During_Update_OthersStillNotified()
    {
        var wd = new WeatherData();

        var selfRemover = new SelfRemovingObserver();
        var recorder = new RecordingObserver();

        wd.RegisterObserver( selfRemover );
        wd.RegisterObserver( recorder );

        // ������ �����: selfRemover ������ ���� ������ Update, recorder ������ �������� �����������
        wd.SetMeasurements( 1.0, 10.0, 760.0 );

        Assert.AreEqual( 1, selfRemover.Received.Count, "Self-removing observer should have received first update." );
        Assert.AreEqual( 1, recorder.Received.Count, "Other observer must still receive update even if one removed itself inside Update." );

        // ������ �����: selfRemover ��� ����� � �� ������ �������� ������ ����������
        wd.SetMeasurements( 2.0, 11.0, 760.0 );

        Assert.AreEqual( 1, selfRemover.Received.Count, "Self-removing observer should not receive updates after it removed itself." );
        Assert.AreEqual( 2, recorder.Received.Count, "Recorder should continue to receive subsequent updates." );
    }

    [Test]
    public void Test_Priority_Order_HigherPriorityNotifiedFirst()
    {
        var wd = new WeatherData();

        var observedOrder = new List<string>();
        var obsA = new OrderingObserver( "A", observedOrder ); // priority 1
        var obsB = new OrderingObserver( "B", observedOrder ); // priority 3
        var obsC = new OrderingObserver( "C", observedOrder ); // priority 2

        wd.RegisterObserver( obsA, priority: 1 );
        wd.RegisterObserver( obsB, priority: 3 );
        wd.RegisterObserver( obsC, priority: 2 );

        wd.SetMeasurements( 5.0, 50.0, 760.0 );

        // ������� �������: B (3), C (2), A (1)
        Assert.AreEqual( 3, observedOrder.Count );
        Assert.AreEqual( "B", observedOrder[ 0 ] );
        Assert.AreEqual( "C", observedOrder[ 1 ] );
        Assert.AreEqual( "A", observedOrder[ 2 ] );
    }

    [Test]
    public void Test_Duplicate_Registration_Is_Ignored()
    {
        var wd = new WeatherData();
        var recorder = new RecordingObserver();

        wd.RegisterObserver( recorder );
        wd.RegisterObserver( recorder ); // ��������� ����������� ������ ���� ���������������

        wd.SetMeasurements( 1.0, 10.0, 760.0 );

        Assert.AreEqual( 1, recorder.Received.Count, "Observer must receive exactly one notification per event even if registered twice." );
    }

    [Test]
    public void Test_Observer_Can_Listen_To_Multiple_Sources_And_Receives_Source_In_Update()
    {
        var indoor = new WeatherData();
        var outdoorPro = new WeatherDataPro();

        var obsIndoor = new SourceAwareObserver<WeatherInfo>();
        var obsOutdoor = new SourceAwareObserver<WeatherInfoPro>();

        // ������������ obsIndoor �� indoor (WeatherInfo)
        indoor.RegisterObserver( obsIndoor );
        // ������������ obsOutdoor �� outdoorPro (WeatherInfoPro)
        outdoorPro.RegisterObserver( obsOutdoor );

        // ����������
        indoor.SetMeasurements( 10.0, 20.0, 760.0 );
        outdoorPro.SetMeasurements( 12.0, 30.0, 758.0, 3.0, 90.0 );

        // ���������, ��� � Update ������ ��� �� ������ ���������, �� ������� ���������
        Assert.AreEqual( 1, obsIndoor.Sources.Count );
        Assert.AreSame( indoor, obsIndoor.Sources[ 0 ] );

        Assert.AreEqual( 1, obsOutdoor.Sources.Count );
        Assert.AreSame( outdoorPro, obsOutdoor.Sources[ 0 ] );
    }
}
