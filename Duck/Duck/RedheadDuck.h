#pragma once

#include "Duck.h"
#include "../Duck/FlyWithWings.h"
#include "../Duck/QuackBehavior.h"
#include "../Duck/MinuetDance.h"

#include <memory>

class RedheadDuck : public Duck
{
public:
    RedheadDuck()
        : Duck(std::make_unique<FlyWithWings>(),
            std::make_unique<QuackBehavior>(),
            std::make_unique<MinuetDance>())
    {
    }

    void Display() const override
    {
        std::cout << "I'm redhead duck" << std::endl;
    }
};