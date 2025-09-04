#pragma once

#include "../Duck/Duck.h"
#include "../Duck/FlyNoWay.h"
#include "../Duck/QuackBehavior.h"
#include "../Duck/NoDance.h"

class ModelDuck : public Duck
{
public:
	ModelDuck()
		: Duck(std::make_unique<FlyNoWay>(),
			std::make_unique<QuackBehavior>(),
			std::make_unique<NoDance>())
	{
	}

	void Display() const override
	{
		std::cout << "I'm model duck" << std::endl;
	}
};
