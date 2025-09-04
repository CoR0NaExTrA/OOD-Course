#pragma once

#include "Duck.h"
#include "../Duck/FlyNoWay.h"
#include "../Duck/SqueakBehavior.h"
#include "../Duck/NoDance.h"

#include <iostream>

class RubberDuck : public Duck
{
public:
	RubberDuck()
		: Duck(std::make_unique<FlyNoWay>(),
			std::make_unique<SqueakBehavior>(),
			std::make_unique<NoDance>())
	{
	}

	void Display() const override
	{
		std::cout << "I'm rubber duck" << std::endl;
	}
};
