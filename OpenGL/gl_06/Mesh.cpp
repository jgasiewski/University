#include "Mesh.h"

size_t Mesh::vertSize = 8;
Shader* Mesh::shader = nullptr;

void Mesh::buffer()
{
	glGenVertexArrays(1, &vao);
	glGenBuffers(1, &vbo);
	glGenBuffers(1, &ebo);

	glBindVertexArray(vao);

	glBindBuffer(GL_ARRAY_BUFFER, vbo);

	glBufferData(GL_ARRAY_BUFFER, vertsNum * vertSize * sizeof(GLfloat), &vertices[0], GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, 3 * trisNum * sizeof(GLuint), &indices[0], GL_STATIC_DRAW);

	// Position attribute
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, vertSize * sizeof(GLfloat), (GLvoid*)0);
	glEnableVertexAttribArray(0);
	// Texture coord attribute
	glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, vertSize * sizeof(GLfloat), (GLvoid*)(3 * sizeof(GLfloat)));
	glEnableVertexAttribArray(1);
	// Normals attribute
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, vertSize * sizeof(GLfloat), (GLvoid*)(5 * sizeof(GLfloat)));
	glEnableVertexAttribArray(2);

	glBindBuffer(GL_ARRAY_BUFFER, 0);

	glBindVertexArray(0);
}

void Mesh::loadTexture(const char* fileName)
{
	glGenTextures(1, &texture);
	glBindTexture(GL_TEXTURE_2D, texture);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

	int width, height;
	unsigned char* image = SOIL_load_image(fileName, &width, &height, 0, SOIL_LOAD_RGB);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, image);
	glGenerateMipmap(GL_TEXTURE_2D);

	SOIL_free_image_data(image);
	glBindTexture(GL_TEXTURE_2D, 0);
}

void Mesh::bindShader(Shader* bindedShader)
{
	shader = bindedShader;
}

Mesh::Mesh(const char* fileName)
{
	vertsNum = 0;
	trisNum = 0;

	buffer();
	loadTexture(fileName);
}

Mesh::~Mesh()
{
	delete vertices;
	delete indices;

	glDeleteVertexArrays(1, &vao);
	glDeleteBuffers(1, &vbo);
	glDeleteBuffers(1, &ebo);

	glDeleteTextures(1, &texture);
}

void Mesh::draw()
{
	shader->Use();

	GLint modelLoc = glGetUniformLocation(shader->Get(), "model");
	glUniformMatrix4fv(modelLoc, 1, GL_FALSE, value_ptr(model));

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, texture);
	glUniform1i(glGetUniformLocation(shader->Get(), "texture"), 0);

	glBindVertexArray(vao);

	glDrawElements(GL_TRIANGLES, 3 * trisNum, GL_UNSIGNED_INT, 0);
	glBindVertexArray(0);
}

void Mesh::setColor(GLfloat r, GLfloat g, GLfloat b)
{
	GLint objectColorLoc = glGetUniformLocation(shader->Get(), "objectColor");
	glUniform3f(objectColorLoc, r, g, b);
}

void Mesh::rotate(float x, float y, float z)
{
	model = glm::rotate(model, glm::radians(x), glm::vec3(1.0f, 0.0f, 0.0f));
	model = glm::rotate(model, glm::radians(y), glm::vec3(0.0f, 1.0f, 0.0f));
	model = glm::rotate(model, glm::radians(z), glm::vec3(0.0f, 0.0f, 1.0f));
}

void Mesh::translate(float x, float y, float z)
{
	model = glm::translate(model, glm::vec3(x, y, z));
}

void Mesh::scale(float x, float y, float z)
{
	model = glm::scale(model, glm::vec3(x, y, z));
}