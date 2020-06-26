#pragma once

#include <GL/glew.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include "Shader.h"

class Camera
{
private:
	GLfloat radius;
	GLfloat angle;

	glm::mat4 view;
	glm::mat4 projection;

	GLfloat shakeTime;
	glm::vec2 offset;
	GLfloat force;
	GLfloat time;

	static Shader* shader;

	void shake(GLfloat dt);
public:
	Camera(GLfloat AspectRatio, Shader* bindedShader, GLfloat radius);

	void update(GLfloat dt, GLfloat normalSpeed, GLfloat angularSpeed);
	void screenShake(GLfloat time, GLfloat force);

	static void bindShader(Shader* bindedShader);
};