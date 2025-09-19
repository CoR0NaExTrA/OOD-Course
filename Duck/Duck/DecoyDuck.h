#pragma once

#include "../Duck/Duck.h"
#include "../Duck/FlyNoWay.h"
#include "../Duck/MuteQuackBehavior.h"
#include "../Duck/NoDance.h"

#include <iostream>
#include <memory>

class DecoyDuck : public Duck
{
public:
	DecoyDuck()
		: Duck(std::make_unique<FlyNoWay>(),
			std::make_unique<MuteQuackBehavior>(),
			std::make_unique<NoDance>())
	{
	}

	void Display() const override
	{
		std::cout << "I'm decoy duck" << std::endl;
	}
};