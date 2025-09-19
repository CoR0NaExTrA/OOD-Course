#pragma once
#include "IFlyBehavior.h"
#include <iostream>

class FlyWithWings : public IFlyBehavior
{
public:
    void Fly() override
    {
        ++m_flightCount;
        std::cout << "I'm flying with wings!! (flight #" << m_flightCount << ")" << std::endl;
    }

    int GetFlightCount() const override
    {
        return m_flightCount;
    }

private:
    int m_flightCount = 0;
};
