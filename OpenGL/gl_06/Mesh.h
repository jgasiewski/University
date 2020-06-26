#pragma once

#include <GL/glew.h>
#include <SOIL.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include "Shader.h"

class Mesh
{
protected:
	static size_t vertSize;
	size_t vertsNum, trisNum;

	GLfloat* vertices;
	GLuint* indices;

	GLuint vao, vbo, ebo;
	GLuint texture;

	static Shader* shader;

	glm::mat4 model;

	void buffer();
	void loadTexture(const char* fileName);

public:
	Mesh(const char* fileName);
	~Mesh();

	static void bindShader(Shader* bindedShader);

	void draw();
	static void setColor(GLfloat r, GLfloat g, GLfloat b);

	void rotate(float x, float y, float z);
	void translate(float x, float y, float z);
	void scale(float x, float y, float z);
};