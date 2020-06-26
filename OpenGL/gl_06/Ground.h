#pragma once

#include "Mesh.h"

class Ground : public Mesh
{
private:
	GLfloat size;
public:
	Ground(const char* fileName, GLfloat radius);
};