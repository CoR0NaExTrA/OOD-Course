#include "../Duck/FlyWithWings.h"
#include "../Duck/QuackBehavior.h"
#include "../Duck/NoDance.h"
#include "../../../Catch/catch.hpp"
#include <memory>
#include <sstream>

TEST_CASE("Dance behavior calls are working", "[dance]")
{
    SECTION("Waltz dance outputs correct message")
    {
        std::stringstream output;
        auto oldCout = std::cout.rdbuf();
        std::cout.rdbuf(output.rdbuf());

        auto dance = behaviors::MakeWaltzDance();
        dance();

        std::cout.rdbuf(oldCout);
        REQUIRE(output.str() == "I'm dancing waltz!\n");
    }

    SECTION("Minuet dance outputs correct message")
    {
        std::stringstream output;
        auto oldCout = std::cout.rdbuf();
        std::cout.rdbuf(output.rdbuf());

        auto dance = behaviors::MakeMinuetDance();
        dance();

        std::cout.rdbuf(oldCout);
        REQUIRE(output.str() == "I'm dancing minuet!\n");
    }

    SECTION("No dance produces no output")
    {
        std::stringstream output;
        auto oldCout = std::cout.rdbuf();
        std::cout.rdbuf(output.rdbuf());

        auto dance = behaviors::MakeNoDance();
        dance();

        std::cout.rdbuf(oldCout);
        REQUIRE(output.str().empty());
    }
}

TEST_CASE("Duck dance method calls behavior", "[duck][dance]")
{
    bool danceCalled = false;
    auto danceBehavior = [&danceCalled]() { danceCalled = true; };
    auto flyBehavior = []() {};
    auto quackBehavior = []() {};

    Duck duck(flyBehavior, quackBehavior, danceBehavior);
    duck.Dance();

    REQUIRE(danceCalled);
}

