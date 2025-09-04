#pragma once

#include "../Duck/IQuackBehavior.h"
#include <iostream>

class QuackBehavior : public IQuackBehavior
{
public:
	void Quack() override
	{
		std::cout << "Quack Quack!!!" << std::endl;
	}
};