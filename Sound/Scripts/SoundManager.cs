using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource defaultSource = default;
    public class _Info
    {
        public bool IsDone;//再生済みかどうかのフラグ
        public int FrameCount;//再生候補になってからの経過フレーム
        public AudioClip Clip;
    }
    private readonly Dictionary<SE_Enum, Queue<_Info>> table = new Dictionary<SE_Enum, Queue<_Info>>();


    //BGM用フィールド
    [SerializeField] private float BGM_Volume = 1.0f;
    [SerializeField] private AudioClip[] IntroBGM_Clips;
    [SerializeField] private AudioClip[] LoopBGM_Clips;
    private AudioSource IntroBGM_Source;
    private AudioSource LoopBGM_Source;
    private BGM_Enum CurrentBGMEnum;
    private Dictionary<BGM_Enum, AudioClip> bgmIntroDictionary = new Dictionary<BGM_Enum, AudioClip>();
    private Dictionary<BGM_Enum, AudioClip> bgmLoopDictionary = new Dictionary<BGM_Enum, AudioClip>();

    //SE用フィールド
    [SerializeField] private float SE_Volume = 1.0f;
    [SerializeField] private AudioClip[] SE_Clips;
    private Dictionary<SE_Enum, AudioClip> seDictionary = new Dictionary<SE_Enum, AudioClip>();
    //再生を遅延させるフレーム数
    private int delayFrameCount = 2;
    //再生を予約できる最大数
    private int maxQuendItemCount = 8;

    public void SetAudioClips(AudioClip[] ses, AudioClip[] intros, AudioClip[] loops)
    {
        SE_Clips = ses;
        IntroBGM_Clips = intros;
        LoopBGM_Clips = loops;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        IntroBGM_Source = gameObject.AddComponent<AudioSource>();
        LoopBGM_Source = gameObject.AddComponent<AudioSource>();
        IntroBGM_Source.loop = false;
        LoopBGM_Source.loop = true;

        if (!this.defaultSource)
        {
            var go = new GameObject("SEManager");
            go.transform.parent = this.transform;
            this.defaultSource = go.AddComponent<AudioSource>();
        }

        foreach (var sound in SE_Clips)
        {
            string name = sound.name.ToUpper().Replace(".WAV", "").Replace(".MP3", "");
            seDictionary.Add((SE_Enum)Enum.Parse(typeof(SE_Enum), name), sound);
        }

        foreach (var intro in IntroBGM_Clips)
        {
            string name = intro.name.ToUpper().Replace(".WAV", "").Replace(".MP3", "").Replace("INTRO", "").ToUpper();
            bgmIntroDictionary.Add((BGM_Enum)Enum.Parse(typeof(BGM_Enum), name), intro);
        }

        foreach (var loop in LoopBGM_Clips)
        {
            string name = loop.name.ToUpper().Replace(".WAV", "").Replace(".MP3", "").Replace("LOOP", "").ToUpper();
            bgmLoopDictionary.Add((BGM_Enum)Enum.Parse(typeof(BGM_Enum), name), loop);
        }

    }

    public void Update()
    {
        //BGM
        if (IntroBGM_Source.clip != null)
        {
            if (!IntroBGM_Source.isPlaying)
            {
                LoopBGM_Source.Play();
                IntroBGM_Source.clip = null;
            }
        }

        //SE
        foreach (var queue in this.table.Values)
        {
            if (queue.Count == 0)
            {
                continue;
            }
            while (true)
            {
                if (queue.Count == 0) break;
                if (queue.Peek().IsDone)
                {
                    queue.Dequeue();
                }
                else
                {
                    break;
                }
            }
            if (queue.Count == 0)
            {
                continue;
            }

            var info = queue.Peek();
            info.FrameCount++;
            if (info.FrameCount > this.delayFrameCount)
            {
                defaultSource.volume = SE_Volume;
                this.defaultSource.PlayOneShot(info.Clip);
                queue.Dequeue();
            }
        }

        if (this.count == 0)
        {
            this.table.Clear();
        }
    }

    public static void PlayBGM(BGM_Enum bgmType)
    {
        instance.InstancePlayBGM(bgmType);
    }

    private void InstancePlayBGM(BGM_Enum bgmType)
    {
        CurrentBGMEnum = bgmType;
        AudioClip intro;
        if (bgmIntroDictionary.TryGetValue(bgmType, out intro))
        {
            if (IntroBGM_Source.clip != null && IntroBGM_Source.clip.Equals(intro)) return;
        }
        AudioClip loop;
        if (bgmLoopDictionary.TryGetValue(bgmType, out loop))
        {
            if (LoopBGM_Source.clip != null && LoopBGM_Source.clip.Equals(loop)) return;
        }

        IntroBGM_Source.clip = intro;
        LoopBGM_Source.clip = loop;

        IntroBGM_Source.volume = BGM_Volume;
        LoopBGM_Source.volume = BGM_Volume;

        if (intro != null)
        {
            IntroBGM_Source.Play();
        }
        else
        {
            LoopBGM_Source.Play();
        }
    }

    public static void StopBGM()
    {
        instance.InstanceStopBGM();
    }

    private void InstanceStopBGM()
    {
        IntroBGM_Source.Stop();
        IntroBGM_Source.clip = null;
        LoopBGM_Source.Stop();
        LoopBGM_Source.clip = null;
    }

    public static void PlaySE(SE_Enum seType)
    {
        instance.InstancePlaySE(seType);
    }

    private void InstancePlaySE(SE_Enum seType)
    {
        AudioClip clip;
        if (seDictionary.TryGetValue(seType, value: out clip))
        {
            var info = new _Info() { FrameCount = 0, Clip = clip, };
            if (!this.table.ContainsKey(seType))
            {
                this.defaultSource.PlayOneShot(clip);
                info.IsDone = true;

                var queue = new Queue<_Info>();
                queue.Enqueue(info);
                this.table[seType] = queue;
            }
            else
            {
                var list = this.table[seType];
                if (list.Count <= this.maxQuendItemCount)
                {
                    this.table[seType].Enqueue(info);
                }
            }
        }
    }

    public static void StopSE()
    {
        instance.InstanceStopSE();
    }

    private void InstanceStopSE()
    {
        defaultSource.Stop();
        table.Clear();
    }

    //有効な要素数の取得
    private int count
    {
        get
        {
            int num = 0;
            foreach (var list in this.table.Values)
            {
                num += list.Count;
            }
            return num;
        }
    }
}

