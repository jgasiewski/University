#pragma once

#include <GL/glew.h>

class Shader
{
	GLuint program;
public:

	// Compile and link shader
	Shader(const GLchar* vertexPath, const GLchar* fragmentPath);
	// Set shader as active
	void Use();
	GLuint Get();
};

