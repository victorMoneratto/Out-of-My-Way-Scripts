using System.Collections;
using OMWGame.Scriptable;
using UnityEngine;

namespace OMWGame {
    public class LaserGunController : MonoBehaviour {
        #region Shooting
        [SerializeField] private LaserGun[] guns;
        [SerializeField] private GameObject laser;
        [SerializeField] private AudioClipRunSet laserSounds;

        [SerializeField] private IntReference shotsPerBurst = 3;
        [SerializeField] private FloatReference timeBetweenShots = 0.1f;
        [SerializeField] private FloatReference cooldownTime = 1f;

        private bool canShoot = true;
        private bool isShooting = false;

        private int currentGunIndex = 0;
        #endregion Shooting

        
        private PlayerInput input;
        private AudioSource audioSrc;

        private void Awake() {
            input = GetComponent<PlayerInput>();
            audioSrc = GetComponent<AudioSource>();
        }

        private void Update() {
            bool wantsShoot = false;
            if (input.Control != null) {
                wantsShoot = input.Control.GetButton("Shoot");
            }

            if (wantsShoot && canShoot) {
                StartCoroutine(Shoot());
            }
        }

        private IEnumerator Shoot() {
            canShoot = false;
            isShooting = true;

            for (int i = 0; i < shotsPerBurst; i++) {
                if (!isShooting) {
                    break;
                }
                
                // Play gun fx
                var gun = guns[currentGunIndex];
                gun.Play();
                audioSrc.PlayOneShot(laserSounds.GetRandom());
                
                // @OMW TODO Pooling/Recycling
                var shot = Instantiate(laser, gun.transform.position, gun.transform.rotation);
                var damager = shot.GetComponent<IDamager>();
                if (damager != null) {
                    damager.Instigator = gameObject;
                }
                
                currentGunIndex = (currentGunIndex + 1) % guns.Length;
                yield return new WaitForSeconds(timeBetweenShots);
            }

            isShooting = false;

            yield return new WaitForSeconds(cooldownTime);
            canShoot = true;
        }
    }
}