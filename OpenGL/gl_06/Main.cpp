#define GLEW_STATIC
#include <GL/glew.h>
#include <GLFW/glfw3.h>

#include "Animation.h"

void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode);

const GLuint WIDTH = 800, HEIGHT = 600;
const GLfloat ASPECT_RATIO = WIDTH / HEIGHT;

bool input = false;

int main()
{
	glfwInit();
	// Set options for GLFW
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

	GLFWwindow* window = glfwCreateWindow(WIDTH, HEIGHT, "OpenGL", nullptr, nullptr);
	if (window == nullptr)
	{
		glfwTerminate();
		return -1;
	}
	glfwMakeContextCurrent(window);

	glfwSetKeyCallback(window, key_callback);

	glewExperimental = GL_TRUE;
	// Initialize GLEW to setup the OpenGL function pointers
	if (glewInit() != GLEW_OK)
	{
		return -1;
	}

	glViewport(0, 0, WIDTH, HEIGHT);
	glEnable(GL_DEPTH_TEST);

	Animation* anim = new Animation(ASPECT_RATIO);

	GLfloat deltaTime = 0.0f;
	GLfloat lastFrame = 0.0f;

	// Main loop
	while (!glfwWindowShouldClose(window))
	{
		glfwPollEvents();
		
		GLfloat currentFrame = (GLfloat)glfwGetTime();

		if (currentFrame - lastFrame > 0)
		{
			deltaTime = currentFrame - lastFrame;
			lastFrame = currentFrame;

			anim->update(deltaTime, input);
		}

		glfwSwapBuffers(window);
	}

	delete anim;

	glfwTerminate();
	return 0;
}

void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode)
{
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		glfwSetWindowShouldClose(window, GL_TRUE);

	if (key == GLFW_KEY_SPACE)
	{
		if (action == GLFW_PRESS/* || action == GLFW_REPEAT*/) input = true;
		else input = false;
	}
}