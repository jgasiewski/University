#include "Camera.h"

Shader* Camera::shader = nullptr;

Camera::Camera(GLfloat AspectRatio, Shader* bindedShader, GLfloat radius)
{
	shakeTime = 0;
	this->radius = radius;
	angle = 0;
	time = 0;

	shader = bindedShader;

	view = glm::translate(view, glm::vec3(0.0f, 0.0f, -3.0f));
	projection = glm::perspective(glm::radians(45.0f), AspectRatio, 0.1f, 1000.0f);

	shader->Use();
	GLint projectionLoc = glGetUniformLocation(shader->Get(), "projection");
	glUniformMatrix4fv(projectionLoc, 1, GL_FALSE, value_ptr(projection));
}

void Camera::shake(GLfloat dt)
{
	time += dt;
	if (shakeTime > 0)
	{
		shakeTime -= dt;
		offset.x = force * sin(50 * time + 10 * dt);
		offset.y = force * cos(50 * time + 6 * dt);
		view = glm::translate(view, glm::vec3(offset.x * cos(glm::radians(angle)), offset.y, 0));
	}
}

void Camera::update(GLfloat dt, GLfloat normalSpeed, GLfloat angularSpeed)
{
	glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	angle += angularSpeed * dt;
	radius += normalSpeed * dt;

	view = glm::translate(glm::mat4(), glm::vec3(0.0f, 0.0f, -radius));
	view = glm::rotate(view, glm::radians(angle), glm::vec3(0, 1, 0));
	shake(dt);


	GLint viewPosLoc = glGetUniformLocation(shader->Get(), "viewPos");
	glUniform3f(viewPosLoc, glm::vec3(view[3]).x, glm::vec3(view[3]).y, glm::vec3(view[3]).z);

	shader->Use();
	GLint viewLoc = glGetUniformLocation(shader->Get(), "view");
	glUniformMatrix4fv(viewLoc, 1, GL_FALSE, value_ptr(view));
}

void Camera::screenShake(GLfloat time, GLfloat force)
{
	if (shakeTime <= 0)
	{
		this->force = force;
		shakeTime = time;
	}
}

void Camera::bindShader(Shader* bindedShader)
{
	shader = bindedShader;
}