#pragma once

#include "Mesh.h"

class Cylindrical : public Mesh
{
public:
	Cylindrical(const char* fileName, GLuint ringsNum, GLuint vertsPerRing, GLfloat* radii, GLfloat* offsets, GLfloat* normAng);
};