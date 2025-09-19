#pragma once

#include "../Duck/IQuackBehavior.h"

class MuteQuackBehavior : public IQuackBehavior
{
public:
	void Quack() override {}
};