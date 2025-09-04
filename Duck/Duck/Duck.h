#pragma once

#include "../Duck/IFlyBehavior.h"
#include "../Duck/IQuackBehavior.h"
#include "../Duck/IDanceBehavior.h"

#include <cassert>
#include <iostream>
#include <memory>
#include <vector>

class Duck
{
public:
	Duck(std::unique_ptr<IFlyBehavior>&& flyBehavior,
		std::unique_ptr<IQuackBehavior>&& quackBehavior,
		std::unique_ptr<IDanceBehavior>&& danceBehavior)
		: m_quackBehavior(std::move(quackBehavior))
		, m_danceBehavior(std::move(danceBehavior))
	{
		assert(m_quackBehavior);
		assert(m_danceBehavior);
		SetFlyBehavior(std::move(flyBehavior));
	}

	void Quack() const
	{
		m_quackBehavior->Quack();
	}

	void Swim()
	{
		std::cout << "I'm swimming" << std::endl;
	}

	void Fly()
	{
		if (m_quackBeforeFlight && CanFly() && IsEvenFlight())
		{
			Quack();
		}

		m_flyBehavior->Fly();

		if (m_quackAfterFlight && CanFly() && IsEvenFlight())
		{
			Quack();
		}
	}

	void Dance()
	{
		m_danceBehavior->Dance();
	}

	void SetFlyBehavior(std::unique_ptr<IFlyBehavior>&& flyBehavior)
	{
		assert(flyBehavior);
		m_flyBehavior = std::move(flyBehavior);
	}

	void SetDanceBehavior(std::unique_ptr<IDanceBehavior>&& danceBehavior)
	{
		assert(danceBehavior);
		m_danceBehavior = std::move(danceBehavior);
	}

	void SetQuackBeforeFlight(bool enabled) { m_quackBeforeFlight = enabled; }
	void SetQuackAfterFlight(bool enabled) { m_quackAfterFlight = enabled; }
	void SetQuackOnEvenFlights(bool enabled) { m_quackOnEvenFlights = enabled; }

	virtual void Display() const = 0;
	virtual ~Duck() = default;

private:
	bool CanFly() const
	{
		return m_flyBehavior->GetFlightsCount() >= 0;
	}

	bool IsEvenFlight() const
	{
		int nextFlightCount = m_flyBehavior->GetFlightsCount() + 1;
		return m_quackOnEvenFlights ? (nextFlightCount % 2 == 0) : (nextFlightCount % 2 != 0);
	}

	bool m_quackBeforeFlight = false;
	bool m_quackAfterFlight = true;
	bool m_quackOnEvenFlights = true;

	std::unique_ptr<IFlyBehavior> m_flyBehavior;
	std::unique_ptr<IQuackBehavior> m_quackBehavior;
	std::unique_ptr<IDanceBehavior> m_danceBehavior;
};