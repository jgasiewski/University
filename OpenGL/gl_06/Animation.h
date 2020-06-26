#pragma once

#include "Mesh.h"
#include "Skybox.h"
#include "Ground.h"
#include "Cylindrical.h"
#pragma once

#include "Camera.h"

class Animation
{
private:
	Camera* camera;
	Mesh* skybox; 
	Mesh* ground;
	Mesh* bell;
	Mesh* heart;
	Mesh* beam;
	Mesh* beamShort;
	Mesh* rope;
	Shader* shader;

	GLfloat bellAcc, bellVel, bellAngle;
	GLfloat heartAcc, heartVel, heartAngle;

	void bellPhys(GLfloat dt, bool input);
	void pendulum(GLfloat &acc, GLfloat &vel, GLfloat &ang, GLfloat extAcc, GLfloat dt, GLfloat inertia);
public:
	Animation(GLfloat aspectRation);
	~Animation();
	void update(GLfloat dt, bool input);
};