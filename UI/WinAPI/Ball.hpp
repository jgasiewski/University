#pragma once

struct Ball
{
	static const int b_speed = 10;
	static const int radius = 10;

	bool active = false;
	bool reported = false;

	float pos[2];
	float speed;

	bool init(bool left, float x, float y)
	{
		if (!active)
		{
			reported = false;
			if (left)
			{
				speed = b_speed;
			}
			else
			{
				speed = -b_speed;
			}

			pos[0] = x;
			pos[1] = y;

			active = true;
			return true;
		}
		
		return false;
	}

	void update()
	{
		pos[0] += speed;
	}

	int left()
	{
		return pos[0] - radius;
	}

	int right()
	{
		return pos[0] + radius;
	}

	int top()
	{
		return -pos[1] + radius;
	}

	int bottom()
	{
		return -pos[1] - radius;
	}
};