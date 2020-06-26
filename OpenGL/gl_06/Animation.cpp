#include "Animation.h"

#include <iostream>
using namespace std;

Animation::Animation(GLfloat aspectRation)
{
	bellAcc = bellVel = heartAcc = heartVel = 0;
	bellAngle = heartAngle = 0;

	const int bellLayers = 13;
	GLfloat bellRadii[bellLayers] = { 0.05f, 0.4f, 0.45f, 0.6f, 0.7f, 0.9f, 0.95f, 1.2f, 1.3f, 1.0f, 0.6f, 0.45f, 0.05f };
	GLfloat bellOffsets[bellLayers] = { 0.0f, -0.1f, -0.15f, -0.25f, -0.5f, -1.5f, -2.0f, -2.2f, -2.4f, -2.5f, -1.0f, -0.8f, -0.5f };
	GLfloat bellNormals[bellLayers] = { 90, 80, 70, 60, 40, 0, 0, 30, 50, -120, -180, -120, -90 };
	const int heartLayers = 7;
	GLfloat heartRadii[heartLayers] = { 0.03f, 0.09f, 0.2f, 0.22f, 0.25f, 0.15f, 0.05f };
	GLfloat heartOffsets[heartLayers] = { 0.02f, -2.3f, -2.5f, -2.6f, -2.7f, -2.8f, -2.9f };
	GLfloat heartNormals[heartLayers] = { 90, 60, 45, 0, -45, -45, -60 };
	const int beamLayers = 4;
	GLfloat beamRadii[beamLayers] = { 0.25f, 0.3f, 0.3f, 0.25f };
	GLfloat beamOffsets[beamLayers] = { 3.1f, 3.0f, -3.0f, -3.1f };
	GLfloat beamNormals[beamLayers] = { 90, 0, 0, -90 };
	const int beamShortLayers = 5;
	GLfloat beamShortRadii[beamShortLayers] = { 0.12f, 0.15f, 0.15f, 0.15f, 0.12f };
	GLfloat beamShortOffsets[beamShortLayers] = { 2.6f, 2.5f, 1.0f, -0.5f, -0.6f };
	GLfloat beamShortNormals[beamShortLayers] = { 90, 0, 0, 0, -90 };
	const int ropeLayers = 2;
	GLfloat ropeRadii[ropeLayers] = { 0.05f, 0.05f };
	GLfloat ropeOffsets[ropeLayers] = { 0.0f, -2.2f };
	GLfloat ropeNormals[ropeLayers] = { 0, 0 };

	shader = new Shader("Shader.vs", "Shader.frag");
	Mesh::bindShader(shader);
	shader->Use();

	GLint lightColorLoc = glGetUniformLocation(shader->Get(), "lightColor");
	glUniform3f(lightColorLoc, 1.0f, 0.9f, 0.8f);
	GLint lightPosLoc = glGetUniformLocation(shader->Get(), "lightPos");
	glUniform3f(lightPosLoc, 5, 10, 5);

	Mesh::setColor(1, 1, 1);

	camera = new Camera(aspectRation, shader, 10);

	skybox = new Skybox("skybox.png", 600);
	ground = new Ground("ground.jpg", 30);
	bell = new Cylindrical("Brass.jpg", bellLayers, 20, bellRadii, bellOffsets, bellNormals);
	heart = new Cylindrical("Brass.jpg", heartLayers, 5, heartRadii, heartOffsets, heartNormals);
	beam = new Cylindrical("Wood.jpg", beamLayers, 8, beamRadii, beamOffsets, beamNormals);
	beamShort = new Cylindrical("Wood.jpg", beamShortLayers, 8, beamShortRadii, beamShortOffsets, beamShortNormals);
	rope = new Cylindrical("Rope.jpg", ropeLayers, 8, ropeRadii, ropeOffsets, ropeNormals);

	bell->translate(0, 2.2, 0);
	heart->translate(0, 2.2, 0);
	beamShort->translate(0, 2.2f, 0);
}

Animation::~Animation()
{
	delete shader;
	delete camera;
	delete skybox;
	delete ground;
	delete bell;
	delete heart;
	delete beam;
	delete beamShort;
	delete rope;
}

void Animation::bellPhys(GLfloat dt, bool input)
{
	pendulum(bellAcc, bellVel, bellAngle, input * 10e4f, dt, 2.5f);
	pendulum(heartAcc, heartVel, heartAngle, 0, dt, 1.2f);

	if (heartAngle < bellAngle - 18.0f)
	{
		camera->screenShake(0.5f, (heartAcc - bellAcc) * 0.005f);
		heartAngle = bellAngle - 18.0f;
		heartAcc = bellAcc;
		heartVel = bellVel;
	}
	else if (heartAngle > bellAngle + 18.0f)
	{
		camera->screenShake(0.5f, (heartAcc - bellAcc) * 0.005f);
		heartAngle = bellAngle + 18.0f;
		heartAcc = bellAcc;
		heartVel = bellVel;
	}
}

void Animation::pendulum(GLfloat &acc, GLfloat &vel, GLfloat &ang, GLfloat extAcc, GLfloat dt, GLfloat inertia)
{
	GLfloat a = 0.25f;

	acc = -a * vel - inertia * ang + extAcc * dt;
	if (acc > 150) acc = 150;
	if (acc < -150) acc = -150;
	vel += acc * dt;
	ang += vel * dt;
}

void Animation::update(GLfloat dt, bool input)
{
	GLint lightModeLoc = glGetUniformLocation(shader->Get(), "lightMode");

	camera->update(dt, 0, 15);

	glUniform3f(lightModeLoc, 1, 0, 0);
	skybox->draw();

	glUniform3f(lightModeLoc, 0.12, 1, 1.25f);
	ground->draw();

	bellPhys(dt, input);
	bell->rotate(bellAngle, 0, 0);
	bell->draw();
	bell->rotate(-bellAngle, 0, 0);

	heart->rotate(heartAngle, 0, 0);
	heart->draw();
	heart->rotate(-heartAngle, 0, 0);

	glUniform3f(lightModeLoc, 0.22, 1, 0.05f);
	beam->translate(-2.0, 0, 0);
	beam->rotate(0, 45, 0);
	beam->draw();
	beam->rotate(0, -45, 0);
	beam->translate(4.0, 0, 0);
	beam->rotate(0, 45, 0);
	beam->draw();
	beam->rotate(0, -45, 0);
	beam->translate(-2.0, 0, 0);

	beam->rotate(0, 0, 90);
	beam->translate(2.2f, 0, 0);
	beam->scale(0.7f, 0.7f, 0.7f);
	beam->rotate(0, -bellAngle, 0);
	beam->draw();
	beam->rotate(0, bellAngle, 0);
	beam->scale(1/0.7f, 1/0.7f, 1/0.7f);
	beam->translate(-2.2, 0, 0);
	beam->rotate(0, 0, -90);

	beamShort->rotate(90 + bellAngle, 0, 0);
	beamShort->draw();
	beamShort->rotate(-90 - bellAngle, 0, 0);

	rope->translate(0, 2.2f - 2.2f * sin(glm::radians(bellAngle)), 2.2f * cos(glm::radians(bellAngle)));
	rope->draw();
	rope->translate(0, -2.2f + 2.2f * sin(glm::radians(bellAngle)), -2.2f * cos(glm::radians(bellAngle)));
}
