#pragma once

struct IFlyBehavior
{
    virtual ~IFlyBehavior() {};
    virtual void Fly() = 0;
    virtual int GetFlightsCount() const = 0;
};