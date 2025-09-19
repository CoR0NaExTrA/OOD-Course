#pragma once

#include "../Duck/IFlyBehavior.h"

class FlyNoWay : public IFlyBehavior
{
public:
    void Fly() override
    {
        std::cout << "I can't fly" << std::endl;
    }
};