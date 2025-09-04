#pragma once

#include "../Duck/IDanceBehavior.h"
#include <iostream>

class NoDance : public IDanceBehavior
{
public:
    void Dance() override {}
};