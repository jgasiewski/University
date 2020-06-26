#pragma once

#include "Mesh.h"

class Skybox : public Mesh
{
private:
	GLfloat size;
public:
	Skybox(const char* fileName, GLfloat radius);
};