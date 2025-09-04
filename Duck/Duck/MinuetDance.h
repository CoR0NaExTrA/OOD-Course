#pragma once

#include "../Duck/IDanceBehavior.h"
#include <iostream>

class MinuetDance : public IDanceBehavior
{
public:
    void Dance() override
    {
        std::cout << "I'm dancing minuet!" << std::endl;
    }
};