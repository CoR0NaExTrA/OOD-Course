#pragma once

struct IQuackBehavior
{
	virtual ~IQuackBehavior() {};
	virtual void Quack() = 0;
};