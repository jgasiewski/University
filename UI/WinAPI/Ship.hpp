#pragma once

#include <cmath>

struct Ship
{
	static const int width = 40;
	static const int height = 100;

	float pos[2];
	float speed;
	int side;
	int windowSize;

	void init(int side, int windowSize)
	{
		speed = 6;
		if (side == 0) pos[0] = 2 * width;
		else pos[0] = windowSize - 2 * width;
		pos[1] = windowSize / 2;
		this->side = side;
		this->windowSize = windowSize;
	}

	void move(float input)
	{
		if (side == 0)
		{
			pos[1] += input * speed;
		}
		else
		{
			pos[1] = input * windowSize;
		}

		if (pos[1] < height / 2)pos[1] = height / 2;
		else if (pos[1] > windowSize - height / 2)pos[1] = windowSize - height / 2;
	}

	bool collide(int x, int y)
	{
		return (abs(x - pos[0]) <= (width / 2) && abs(y - pos[1]) <= (height / 2));
	}

	int left()
	{
		return pos[0] - width / 2;
	}

	int right()
	{
		return pos[0] + width / 2;
	}

	int top()
	{
		return - pos[1] + height / 2;
	}

	int bottom()
	{
		return - pos[1] - height / 2;
	}
};