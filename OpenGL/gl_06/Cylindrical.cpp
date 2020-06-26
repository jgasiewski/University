#include "Cylindrical.h"

#include <glm/gtx/rotate_vector.hpp>

#include <iostream>
using namespace std;

Cylindrical::Cylindrical(const char* fileName, GLuint ringsNum, GLuint vertsPerRing, GLfloat* radii, GLfloat* offsets, GLfloat* normAng) : Mesh(fileName)
{
	vertsNum = ringsNum * vertsPerRing;
	trisNum = 2 * ((vertsPerRing - 2) + (ringsNum - 1) * vertsPerRing);

	vertices = new GLfloat[vertSize * vertsNum];
	indices = new GLuint[3 * trisNum];

	GLfloat yTotal = 0;

	for (GLuint i = 1; i < ringsNum; ++i)
	{
		yTotal += offsets[i];
	}

	for (GLuint i = 0; i < ringsNum; ++i)
	{
		GLfloat y = offsets[0];

		for (GLuint j = 0; j < vertsPerRing; ++j)
		{
			GLuint loc = (i * vertsPerRing + j) * vertSize;
			GLfloat angle = glm::two_pi<float>() * j / (float)vertsPerRing;

			vertices[loc] = cos(angle) * radii[i];
			vertices[loc + 1] = offsets[i];
			vertices[loc + 2] = sin(angle) * radii[i];

			vertices[loc + 3] = 1 - 2 * abs((float)j - vertsPerRing/2) / (float)(vertsPerRing - 1);
			vertices[loc + 4] = abs(y - offsets[i]) / (float)(ringsNum - 1);

			glm::vec3 norm = glm::vec3(1, 0, 0);
			norm = glm::rotate(norm, glm::radians(normAng[i]), glm::vec3(0, 0, 1));
			norm = glm::rotate(norm, -angle, glm::vec3(0, 1, 0));
			vertices[loc + 5] = norm.x;
			vertices[loc + 6] = norm.y;
			vertices[loc + 7] = norm.z;
			
		}
		y += offsets[i];
	}

	for (GLuint i = 0; i < vertsPerRing - 2; ++i)
	{
		indices[3 * i] = 0;
		indices[3 * i + 1] = i + 1;
		indices[3 * i + 2] = i + 2;
	}
	for (GLuint i = 0; i < vertsPerRing - 2; ++i)
	{
		indices[3 * (i + vertsPerRing - 2)] = (ringsNum - 1) * vertsPerRing;
		indices[3 * (i + vertsPerRing - 2) + 1] = (ringsNum - 1) * vertsPerRing + i + 1;
		indices[3 * (i + vertsPerRing - 2) + 2] = (ringsNum - 1) * vertsPerRing + i + 2;
	}

	for (GLuint i = 0; i < ringsNum - 1; ++i)
	{
		for (GLuint j = 0; j < vertsPerRing; ++j)
		{
			GLuint offset = 6 * ((i * vertsPerRing + j) + (vertsPerRing - 2));
			indices[offset] = i * vertsPerRing + j;
			indices[offset + 1] = i * vertsPerRing + vertsPerRing + j;
			indices[offset + 2] = i * vertsPerRing + vertsPerRing + ((j + 1) % vertsPerRing);
			indices[offset + 3] = i * vertsPerRing + ((j + 1) % vertsPerRing);
			indices[offset + 4] = i * vertsPerRing + j;
			indices[offset + 5] = i * vertsPerRing + vertsPerRing + ((j + 1) % vertsPerRing);
		}
	}

	buffer();
	loadTexture(fileName);
}