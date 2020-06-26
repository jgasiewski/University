#include "Skybox.h"

Skybox::Skybox(const char* fileName, GLfloat radius) : Mesh(fileName)
{
	size = radius;

	vertsNum = 24;
	trisNum = 8;

	GLfloat q = 0.25f;

	vertices = new GLfloat[vertSize * vertsNum]
	{
		-0.5f * size, -0.5f * size, -0.5f * size, q, 2 * q, 0, 0, 1,
		0.5f * size, -0.5f * size, -0.5f * size, 0, 2 * q, 0, 0, 1,
		0.5f * size, 0.5f * size, -0.5f * size, 0, 3 * q, 0, 0, 1,
		0.5f * size, 0.5f * size, -0.5f * size, 0, 3 * q, 0, 0, 1,
		-0.5f * size, 0.5f * size, -0.5f * size, q, 3 * q, 0, 0, 1,
		-0.5f * size, -0.5f * size, -0.5f * size, q, 2 * q, 0, 0, 1,

		-0.5f * size, 0.5f * size, 0.5f * size, 2 * q, 3 * q, 1, 0, 0,
		-0.5f * size, 0.5f * size, -0.5f * size, q, 3 * q, 1, 0, 0,
		-0.5f * size, -0.5f * size, -0.5f * size, q, 2 * q, 1, 0, 0,
		-0.5f * size, -0.5f * size, -0.5f * size, q, 2 * q, 1, 0, 0,
		-0.5f * size, -0.5f * size, 0.5f * size, 2 * q, 2 * q, 1, 0, 0,
		-0.5f * size, 0.5f * size, 0.5f * size, 2 * q, 3 * q, 1, 0, 0,

		-0.5f * size, -0.5f * size, 0.5f * size, 2 * q, 2 * q, 0, 0, -1,
		0.5f * size, -0.5f * size, 0.5f * size, 3 * q, 2 * q, 0, 0, -1,
		0.5f * size, 0.5f * size, 0.5f * size, 3 * q, 3 * q, 0, 0, -1,
		0.5f * size, 0.5f * size, 0.5f * size, 3 * q, 3 * q, 0, 0, -1,
		-0.5f * size, 0.5f * size, 0.5f * size, 2 * q, 3 * q, 0, 0, -1,
		-0.5f * size, -0.5f * size, 0.5f * size, 2 * q, 2 * q, 0, 0, -1,

		0.5f * size, 0.5f * size, 0.5f * size, 3 * q, 3 * q, -1, 0, 0,
		0.5f * size, 0.5f * size, -0.5f * size, 4 * q, 3 * q, -1, 0, 0,
		0.5f * size, -0.5f * size, -0.5f * size, 4 * q, 2 * q, -1, 0, 0,
		0.5f * size, -0.5f * size, -0.5f * size, 4 * q, 2 * q, -1, 0, 0,
		0.5f * size, -0.5f * size, 0.5f * size, 3 * q, 2 * q, -1, 0, 0,
		0.5f * size, 0.5f * size, 0.5f * size, 3 * q, 3 * q, -1, 0, 0,
	};

	indices = new GLuint[3 * trisNum];

	for (int i = 0; i < 3 * trisNum; ++i)indices[i] = i;

	buffer();
	loadTexture(fileName);
}