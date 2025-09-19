#pragma once

class IFlyBehavior
{
public:
    virtual ~IFlyBehavior() = default;
    virtual void Fly() = 0;

    virtual int GetFlightCount() const { return 0; }
};
