#include "pch.h"

#include <gtest/gtest.h>
#include "../Duck/Duck.h"
#include "../Duck/FlyNoWay.h"
#include "../Duck/QuackBehavior.h"
#include "MockDanceBehavior.h"

// �������� ����� ����
class TestDuck : public Duck
{
public:
    TestDuck(std::unique_ptr<IFlyBehavior>&& flyBehavior,
        std::unique_ptr<IQuackBehavior>&& quackBehavior,
        std::unique_ptr<IDanceBehavior>&& danceBehavior)
        : Duck(std::move(flyBehavior), std::move(quackBehavior), std::move(danceBehavior))
    {
    }

    void Display() const override
    {
        // ������ ���������� ��� ������
    }
};

class DuckDanceTest : public ::testing::Test
{
protected:
    void SetUp() override
    {
        mockDanceBehavior = new MockDanceBehavior();
        danceBehaviorPtr.reset(mockDanceBehavior);

        duck = std::make_unique<TestDuck>(
            std::make_unique<FlyNoWay>(),
            std::make_unique<QuackBehavior>(),
            std::move(danceBehaviorPtr)
        );
    }

    void TearDown() override
    {
        mockDanceBehavior = nullptr; // ��������� ������ ����������� unique_ptr
    }

    MockDanceBehavior* mockDanceBehavior;
    std::unique_ptr<IDanceBehavior> danceBehaviorPtr;
    std::unique_ptr<TestDuck> duck;
};

// ���� 1: ���������, ��� Dance �������� ���������
TEST_F(DuckDanceTest, DanceCallsDanceBehavior)
{
    int callCount = 0;
    mockDanceBehavior->SetOnDanceCallback([&callCount]() {
        callCount++;
        });

    duck->Dance();

    EXPECT_EQ(callCount, 1);
    EXPECT_EQ(mockDanceBehavior->GetDanceCallCount(), 1);
}

// ���� 2: ��������� ������������ �����
TEST_F(DuckDanceTest, DanceCalledMultipleTimes)
{
    int callCount = 0;
    mockDanceBehavior->SetOnDanceCallback([&callCount]() {
        callCount++;
        });

    duck->Dance();
    duck->Dance();
    duck->Dance();

    EXPECT_EQ(callCount, 3);
    EXPECT_EQ(mockDanceBehavior->GetDanceCallCount(), 3);
}

// ���� 3: ��������� ����� ���������
TEST_F(DuckDanceTest, SetDanceBehaviorChangesBehavior)
{
    auto newMockDanceBehavior = new MockDanceBehavior();
    std::unique_ptr<IDanceBehavior> newDanceBehaviorPtr(newMockDanceBehavior);

    int newCallCount = 0;
    newMockDanceBehavior->SetOnDanceCallback([&newCallCount]() {
        newCallCount++;
        });

    duck->SetDanceBehavior(std::move(newDanceBehaviorPtr));
    duck->Dance();

    EXPECT_EQ(newCallCount, 1);
}

// ���� 4: ���������, ��� ��������� �� ��������� ��������
TEST_F(DuckDanceTest, DefaultDanceBehaviorWorks)
{
    // �� ������������� callback - ���������, ��� �� ������
    EXPECT_NO_THROW({
        duck->Dance();
        });

    EXPECT_EQ(mockDanceBehavior->GetDanceCallCount(), 1);
}

// ���� 5: ��������� ����������� � ������� �����������
TEST(DuckConstructorTest, ConstructorWithDifferentBehaviors)
{
    auto mockDance = std::make_unique<MockDanceBehavior>();
    auto* mockPtr = mockDance.get();

    TestDuck testDuck(
        std::make_unique<FlyNoWay>(),
        std::make_unique<QuackBehavior>(),
        std::move(mockDance)
    );

    testDuck.Dance();
    EXPECT_EQ(mockPtr->GetDanceCallCount(), 1);
}