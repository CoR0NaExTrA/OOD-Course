#pragma once

#include "../Duck/IDanceBehavior.h"
#include <iostream>
#include <functional>

class MockDanceBehavior : public IDanceBehavior
{
public:
    MockDanceBehavior() = default;

    void Dance() override
    {
        danceCallCount++;
        if (onDanceCallback)
        {
            onDanceCallback();
        }
    }

    void SetOnDanceCallback(std::function<void()> callback)
    {
        onDanceCallback = callback;
    }

    int GetDanceCallCount() const
    {
        return danceCallCount;
    }

    void Reset()
    {
        danceCallCount = 0;
        onDanceCallback = nullptr;
    }

private:
    int danceCallCount = 0;
    std::function<void()> onDanceCallback;
};