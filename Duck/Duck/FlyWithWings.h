#pragma once

#include "../Duck/IFlyBehavior.h"
#include <iostream>

class FlyWithWings : public IFlyBehavior
{
public:
    FlyWithWings() : m_flightsCount(0) {}

    void Fly() override
    {
        ++m_flightsCount;
        std::cout << "I'm flying with wings!! Flight #" << m_flightsCount << std::endl;
    }

    int GetFlightsCount() const
    {
        return m_flightsCount;
    }

private:
    int m_flightsCount;
};