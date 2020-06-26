#include "Ground.h"

Ground::Ground(const char* fileName, GLfloat radius) : Mesh(fileName)
{
	size = radius;

	vertsNum = 4;
	trisNum = 2;

	GLfloat y = -3;
	GLfloat repeats = 5;

	vertices = new GLfloat[vertSize * vertsNum]
	{
		0.5f * size, y, 0.5f * size, repeats, repeats, 0, 1, 0,
		0.5f * size, y, -0.5f * size, 0, repeats, 0, 1, 0,

		-0.5f * size, y, -0.5f * size, 0, 0, 0, 1, 0,
		-0.5f * size, y, 0.5f * size, repeats, 0, 0, 1, 0,
	};

	indices = new GLuint[3 * trisNum]
	{
		0, 1, 2,
		0, 2, 3
	};

	buffer();
	loadTexture(fileName);
}